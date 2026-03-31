namespace TimeTrace.Platform.Abstractions.Screenshot;

/// <summary>
/// Platform-agnostic screenshot capture service.
/// Implementations handle OS-specific capture mechanisms.
/// </summary>
public interface IScreenshotCaptureService
{
    /// <summary>
    /// Captures a screenshot of the currently active window.
    /// </summary>
    /// <param name="outputPath">Path where screenshot should be saved.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing success status, file path, and window metadata.</returns>
    Task<ScreenshotResult> CaptureActiveWindowAsync(
        string outputPath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Captures a screenshot of the entire screen/display.
    /// </summary>
    /// <param name="outputPath">Path where screenshot should be saved.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing success status and file path.</returns>
    Task<ScreenshotResult> CaptureFullScreenAsync(
        string outputPath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Indicates whether this capture service is available on the current system.
    /// </summary>
    bool IsAvailable { get; }
}
