using TimeTrace.Platform.Abstractions.Screenshot;

namespace TimeTrace.Platform.Windows.Screenshot;

/// <summary>
/// Windows implementation of active window detection service.
/// TODO: Implement using Win32 APIs (GetForegroundWindow, GetWindowText, etc.)
/// </summary>
public sealed class WindowsActiveWindowService : IActiveWindowService
{
    /// <inheritdoc />
    public bool IsSupported => false; // TODO: Return true when implemented

    /// <inheritdoc />
    public Task<ActiveWindowInfo?> GetActiveWindowAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Implement Windows active window detection using:
        // - GetForegroundWindow() to get active window handle
        // - GetWindowText() to get window title
        // - GetWindowThreadProcessId() to get process ID
        // - Process.GetProcessById() to get process name
        throw new PlatformNotSupportedException(
            "Windows active window detection is not yet implemented. " +
            "This stub exists for future development.");
    }
}
