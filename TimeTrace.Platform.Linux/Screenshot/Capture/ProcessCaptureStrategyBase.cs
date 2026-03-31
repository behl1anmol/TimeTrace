using Microsoft.Extensions.Logging;
using TimeTrace.Platform.Abstractions.Screenshot;
using SysProcess = System.Diagnostics.Process;
using SysProcessStartInfo = System.Diagnostics.ProcessStartInfo;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace TimeTrace.Platform.Linux.Screenshot.Capture;

/// <summary>
/// Base class for process-based screenshot capture strategies.
/// </summary>
public abstract class ProcessCaptureStrategyBase : ICaptureStrategy
{
    protected readonly ILogger Logger;
    private readonly Lazy<bool> _isAvailable;

    protected ProcessCaptureStrategyBase(ILogger logger)
    {
        Logger = logger;
        _isAvailable = new Lazy<bool>(CheckToolAvailable);
    }

    public abstract string Name { get; }
    public abstract int Priority { get; }
    public bool IsAvailable => _isAvailable.Value;

    protected abstract string ToolName { get; }
    protected abstract string GetActiveWindowArguments(string outputPath);
    protected abstract string GetFullScreenArguments(string outputPath);

    public async Task<ScreenshotResult> CaptureActiveWindowAsync(string outputPath, CancellationToken cancellationToken)
    {
        return await CaptureAsync(GetActiveWindowArguments(outputPath), outputPath, cancellationToken);
    }

    public async Task<ScreenshotResult> CaptureFullScreenAsync(string outputPath, CancellationToken cancellationToken)
    {
        return await CaptureAsync(GetFullScreenArguments(outputPath), outputPath, cancellationToken);
    }

    protected virtual async Task<ScreenshotResult> CaptureAsync(string arguments, string outputPath, CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Ensure output directory exists
            var dir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var psi = new SysProcessStartInfo(ToolName, arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = SysProcess.Start(psi);
            if (process is null)
            {
                return ScreenshotResult.Failed($"Failed to start {ToolName}");
            }

            var errorTask = process.StandardError.ReadToEndAsync(ct);
            await process.WaitForExitAsync(ct);
            stopwatch.Stop();

            if (process.ExitCode != 0)
            {
                var error = await errorTask;
                Logger.LogDebug("{Tool} failed with exit code {ExitCode}: {Error}",
                    ToolName, process.ExitCode, error);
                return ScreenshotResult.Failed($"{ToolName} exited with code {process.ExitCode}: {error}");
            }

            // Verify file was created
            if (!File.Exists(outputPath))
            {
                return ScreenshotResult.Failed($"{ToolName} completed but file not found at {outputPath}");
            }

            Logger.LogDebug("{Tool} captured screenshot to {Path} in {Duration}ms",
                ToolName, outputPath, stopwatch.ElapsedMilliseconds);

            return ScreenshotResult.Succeeded(outputPath, null, stopwatch.Elapsed);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Logger.LogError(ex, "Error capturing with {Tool}", ToolName);
            return ScreenshotResult.Failed($"{ToolName} error: {ex.Message}");
        }
    }

    private bool CheckToolAvailable()
    {
        try
        {
            var psi = new SysProcessStartInfo("which", ToolName)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = SysProcess.Start(psi);
            process?.WaitForExit(1000);
            var available = process?.ExitCode == 0;

            Logger.LogDebug("{Tool} availability: {Available}", ToolName, available);
            return available;
        }
        catch
        {
            return false;
        }
    }
}
