using TimeTrace.Platform.Abstractions.Screenshot;

namespace TimeTrace.Platform.Windows.Screenshot;

/// <summary>
/// Windows implementation of screenshot capture service.
/// TODO: Implement using Windows Graphics Capture API or similar for Windows 10+
/// </summary>
public sealed class WindowsScreenshotCaptureService : IScreenshotCaptureService
{
    /// <inheritdoc />
    public bool IsAvailable => false; // TODO: Return true when implemented

    /// <inheritdoc />
    public Task<ScreenshotResult> CaptureActiveWindowAsync(string outputPath, CancellationToken cancellationToken = default)
    {
        // TODO: Implement Windows screenshot capture using:
        // - Windows.Graphics.Capture API (Windows 10 1803+)
        // - Graphics.CopyFromScreen for legacy support
        // - Or third-party library like ScreenCapture.NET
        throw new PlatformNotSupportedException(
            "Windows screenshot capture is not yet implemented. " +
            "This stub exists for future development.");
    }

    /// <inheritdoc />
    public Task<ScreenshotResult> CaptureFullScreenAsync(string outputPath, CancellationToken cancellationToken = default)
    {
        // TODO: Implement full screen capture
        throw new PlatformNotSupportedException(
            "Windows screenshot capture is not yet implemented. " +
            "This stub exists for future development.");
    }
}
