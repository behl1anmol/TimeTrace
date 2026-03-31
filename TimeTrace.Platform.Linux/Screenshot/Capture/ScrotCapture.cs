using Microsoft.Extensions.Logging;

namespace TimeTrace.Platform.Linux.Screenshot.Capture;

/// <summary>
/// Screenshot capture using scrot.
/// X11 only - lightweight and widely available.
/// </summary>
public class ScrotCapture : ProcessCaptureStrategyBase
{
    public ScrotCapture(ILogger<ScrotCapture> logger) : base(logger)
    {
    }

    public override string Name => "scrot";
    public override int Priority => 80; // Lower than gnome-screenshot
    protected override string ToolName => "scrot";

    protected override string GetActiveWindowArguments(string outputPath)
    {
        // -u captures the focused window, -o overwrites existing file
        return $"-u -o \"{outputPath}\"";
    }

    protected override string GetFullScreenArguments(string outputPath)
    {
        // -o overwrites existing file
        return $"-o \"{outputPath}\"";
    }
}
