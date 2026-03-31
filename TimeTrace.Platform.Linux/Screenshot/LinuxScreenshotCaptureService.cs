using Microsoft.Extensions.Logging;
using TimeTrace.Platform.Abstractions.Screenshot;
using TimeTrace.Platform.Linux.Screenshot.Capture;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace TimeTrace.Platform.Linux.Screenshot;

/// <summary>
/// Linux screenshot capture service with fallback chain.
/// Tries multiple capture strategies in order of priority.
/// </summary>
public class LinuxScreenshotCaptureService : IScreenshotCaptureService
{
    private readonly IEnumerable<ICaptureStrategy> _captureStrategies;
    private readonly IActiveWindowService _activeWindowService;
    private readonly ILogger<LinuxScreenshotCaptureService> _logger;

    public LinuxScreenshotCaptureService(
        IEnumerable<ICaptureStrategy> captureStrategies,
        IActiveWindowService activeWindowService,
        ILogger<LinuxScreenshotCaptureService> logger)
    {
        // Order by priority (highest first)
        _captureStrategies = captureStrategies.OrderByDescending(s => s.Priority).ToList();
        _activeWindowService = activeWindowService;
        _logger = logger;

        _logger.LogInformation("Initialized with {Count} capture strategies: {Strategies}",
            _captureStrategies.Count(),
            string.Join(", ", _captureStrategies.Select(s => $"{s.Name} (priority={s.Priority}, available={s.IsAvailable})")));
    }

    public bool IsAvailable => _captureStrategies.Any(s => s.IsAvailable);

    public async Task<ScreenshotResult> CaptureActiveWindowAsync(
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        // Get active window info first
        var windowInfo = await _activeWindowService.GetActiveWindowAsync(cancellationToken);
        if (windowInfo is null)
        {
            _logger.LogWarning("No active window detected - will capture anyway");
        }
        else
        {
            _logger.LogDebug("Active window: {ProcessName} - {Title}",
                windowInfo.ProcessName, windowInfo.WindowTitle);
        }

        // Try each available strategy
        foreach (var strategy in _captureStrategies.Where(s => s.IsAvailable))
        {
            try
            {
                _logger.LogDebug("Attempting capture with {Strategy}", strategy.Name);

                var result = await strategy.CaptureActiveWindowAsync(outputPath, cancellationToken);

                if (result.Success)
                {
                    stopwatch.Stop();
                    _logger.LogInformation("Screenshot captured successfully with {Strategy} in {Duration}ms",
                        strategy.Name, stopwatch.ElapsedMilliseconds);

                    return result with
                    {
                        WindowInfo = windowInfo,
                        Duration = stopwatch.Elapsed
                    };
                }

                _logger.LogDebug("Strategy {Strategy} failed: {Error}", strategy.Name, result.ErrorMessage);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Strategy {Strategy} threw exception", strategy.Name);
            }
        }

        stopwatch.Stop();
        _logger.LogError("All capture strategies failed");

        return ScreenshotResult.Failed("All capture strategies failed. " +
            "Ensure at least one of: gnome-screenshot, scrot, or grim is installed.");
    }

    public async Task<ScreenshotResult> CaptureFullScreenAsync(
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        foreach (var strategy in _captureStrategies.Where(s => s.IsAvailable))
        {
            try
            {
                _logger.LogDebug("Attempting full screen capture with {Strategy}", strategy.Name);

                var result = await strategy.CaptureFullScreenAsync(outputPath, cancellationToken);

                if (result.Success)
                {
                    stopwatch.Stop();
                    _logger.LogInformation("Full screen captured with {Strategy} in {Duration}ms",
                        strategy.Name, stopwatch.ElapsedMilliseconds);

                    return result with { Duration = stopwatch.Elapsed };
                }

                _logger.LogDebug("Strategy {Strategy} failed: {Error}", strategy.Name, result.ErrorMessage);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Strategy {Strategy} threw exception", strategy.Name);
            }
        }

        stopwatch.Stop();
        return ScreenshotResult.Failed("All capture strategies failed for full screen.");
    }
}
