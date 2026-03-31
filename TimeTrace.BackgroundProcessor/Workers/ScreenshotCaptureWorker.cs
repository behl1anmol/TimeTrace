using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TimeTrace.BackgroundProcessor.Services;

namespace TimeTrace.BackgroundProcessor.Workers;

/// <summary>
/// Background worker that periodically captures screenshots.
/// </summary>
public class ScreenshotCaptureWorker : BackgroundService
{
    private readonly ICaptureOrchestrationService _captureService;
    private readonly ICaptureConfigurationService _configService;
    private readonly ILogger<ScreenshotCaptureWorker> _logger;

    private int _consecutiveFailures = 0;
    private const int MaxConsecutiveFailures = 5;
    private const int CleanupIntervalMinutes = 30;
    private DateTime _lastCleanup = DateTime.MinValue;

    public ScreenshotCaptureWorker(
        ICaptureOrchestrationService captureService,
        ICaptureConfigurationService configService,
        ILogger<ScreenshotCaptureWorker> logger)
    {
        _captureService = captureService;
        _configService = configService;
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Screenshot Capture Worker starting");

        // Ensure default configuration exists
        _configService.EnsureDefaultConfiguration();

        var settings = _configService.GetCurrentSettings();
        _logger.LogInformation("Capture settings: Interval={Interval}s, Enabled={Enabled}, TempPath={TempPath}",
            settings.IntervalSeconds, settings.Enabled, settings.TempFolderPath);

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Screenshot Capture Worker started");

        // Small initial delay to let the system settle
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Reload settings each iteration (allows runtime config changes)
                var settings = _configService.GetCurrentSettings();

                if (!settings.Enabled)
                {
                    _logger.LogDebug("Capture is disabled, waiting...");
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                    continue;
                }

                // Perform capture
                var result = await _captureService.CaptureAndPersistAsync(stoppingToken);

                if (result.Success)
                {
                    _consecutiveFailures = 0;
                    _logger.LogDebug("Capture completed successfully");
                }
                else
                {
                    _consecutiveFailures++;
                    _logger.LogWarning("Capture failed ({Failures}/{Max}): {Error}",
                        _consecutiveFailures, MaxConsecutiveFailures, result.ErrorMessage);

                    if (_consecutiveFailures >= MaxConsecutiveFailures)
                    {
                        _logger.LogError("Too many consecutive failures, backing off for 5 minutes");
                        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                        _consecutiveFailures = 0;
                    }
                }

                // Periodic cleanup
                if (DateTime.UtcNow - _lastCleanup > TimeSpan.FromMinutes(CleanupIntervalMinutes))
                {
                    _captureService.CleanupTempFolder(settings.MaxTempFolderSizeMB);
                    _lastCleanup = DateTime.UtcNow;
                }

                // Wait for next capture
                await Task.Delay(settings.CaptureInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Screenshot Capture Worker stopping gracefully");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in capture loop");
                _consecutiveFailures++;

                // Exponential backoff on errors
                var backoffSeconds = Math.Min(300, Math.Pow(2, _consecutiveFailures) * 10);
                await Task.Delay(TimeSpan.FromSeconds(backoffSeconds), stoppingToken);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Screenshot Capture Worker stopping");
        await base.StopAsync(cancellationToken);
        _logger.LogInformation("Screenshot Capture Worker stopped");
    }
}
