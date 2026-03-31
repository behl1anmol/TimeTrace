using Microsoft.Extensions.Logging;

namespace TimeTrace.Platform.Linux.Screenshot.Capture;

/// <summary>
/// Screenshot capture using grim.
/// Wayland native (wlroots-based compositors).
/// </summary>
public class GrimCapture : ProcessCaptureStrategyBase
{
    private readonly Lazy<bool> _isWayland;

    public GrimCapture(ILogger<GrimCapture> logger) : base(logger)
    {
        _isWayland = new Lazy<bool>(() =>
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY")));
    }

    public override string Name => "grim";
    public override int Priority => 90; // High priority for Wayland
    protected override string ToolName => "grim";

    // Only available if we're on Wayland AND grim is installed
    public new bool IsAvailable => _isWayland.Value && base.IsAvailable;

    protected override string GetActiveWindowArguments(string outputPath)
    {
        // grim doesn't directly support active window capture
        // It needs slurp for region selection or full screen
        // For active window, we'll capture full screen and note this limitation
        Logger.LogDebug("grim doesn't support active window capture directly - capturing full screen");
        return $"\"{outputPath}\"";
    }

    protected override string GetFullScreenArguments(string outputPath)
    {
        return $"\"{outputPath}\"";
    }
}
