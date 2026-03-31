using TimeTrace.Platform.Abstractions.Screenshot;

namespace TimeTrace.Platform.Linux.Screenshot.Capture;

/// <summary>
/// Interface for screenshot capture strategies.
/// Each strategy wraps a specific Linux screenshot tool.
/// </summary>
public interface ICaptureStrategy
{
    /// <summary>Tool/method name for logging.</summary>
    string Name { get; }

    /// <summary>Priority (higher = tried first).</summary>
    int Priority { get; }

    /// <summary>Whether this tool is available on the system.</summary>
    bool IsAvailable { get; }

    /// <summary>Captures the active window screenshot.</summary>
    /// <param name="outputPath">Path where screenshot should be saved.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the capture operation.</returns>
    Task<ScreenshotResult> CaptureActiveWindowAsync(string outputPath, CancellationToken cancellationToken);

    /// <summary>Captures full screen screenshot.</summary>
    /// <param name="outputPath">Path where screenshot should be saved.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the capture operation.</returns>
    Task<ScreenshotResult> CaptureFullScreenAsync(string outputPath, CancellationToken cancellationToken);
}
