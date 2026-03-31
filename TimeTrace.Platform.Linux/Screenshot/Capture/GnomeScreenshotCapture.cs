using Microsoft.Extensions.Logging;

namespace TimeTrace.Platform.Linux.Screenshot.Capture;

/// <summary>
/// Screenshot capture using gnome-screenshot.
/// Works on both X11 and Wayland via GNOME's portal.
/// </summary>
public class GnomeScreenshotCapture : ProcessCaptureStrategyBase
{
    public GnomeScreenshotCapture(ILogger<GnomeScreenshotCapture> logger) : base(logger)
    {
    }

    public override string Name => "gnome-screenshot";
    public override int Priority => 100; // Highest priority - most compatible
    protected override string ToolName => "gnome-screenshot";

    protected override string GetActiveWindowArguments(string outputPath)
    {
        // -w captures the active window, -f specifies output file
        return $"-w -f \"{outputPath}\"";
    }

    protected override string GetFullScreenArguments(string outputPath)
    {
        // -f specifies output file (full screen is default)
        return $"-f \"{outputPath}\"";
    }
}
