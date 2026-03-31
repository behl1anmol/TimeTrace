using System.Diagnostics;

namespace timetrace.ui.avalonia.Services;

/// <summary>
/// Service for managing the system tray icon on Linux.
/// NOTE: Full implementation requires libayatana-appindicator or similar.
/// This is a placeholder that can be extended with DBus integration.
/// </summary>
public interface ITrayIconService : IDisposable
{
    bool IsSupported { get; }
    void Show();
    void Hide();
    void SetToolTip(string text);
    event EventHandler? Clicked;
    event EventHandler? ShowRequested;
    event EventHandler? ExitRequested;
}

public class LinuxTrayIconService : ITrayIconService
{
    private bool _isVisible;
    
    public bool IsSupported => CheckTraySupport();
    
    public event EventHandler? Clicked;
    public event EventHandler? ShowRequested;
    public event EventHandler? ExitRequested;

    public void Show()
    {
        if (!IsSupported)
        {
            Debug.WriteLine("System tray is not supported on this Linux system.");
            return;
        }
        
        _isVisible = true;
        Debug.WriteLine("Tray icon shown (placeholder - DBus integration needed)");
        
        // TODO: Implement actual tray icon via:
        // 1. libayatana-appindicator3 via P/Invoke
        // 2. DBus StatusNotifierItem protocol
        // 3. Third-party package like Avalonia.Notification
    }

    public void Hide()
    {
        _isVisible = false;
        Debug.WriteLine("Tray icon hidden");
    }

    public void SetToolTip(string text)
    {
        Debug.WriteLine($"Tray tooltip set: {text}");
    }

    public void Dispose()
    {
        Hide();
        GC.SuppressFinalize(this);
    }

    private static bool CheckTraySupport()
    {
        // Check for common Linux desktop environments with tray support
        var desktopSession = Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP");
        var supportedDesktops = new[] { "GNOME", "KDE", "XFCE", "CINNAMON", "MATE", "Unity", "Budgie", "LXQt" };
        
        if (string.IsNullOrEmpty(desktopSession))
            return false;
            
        return supportedDesktops.Any(d => 
            desktopSession.Contains(d, StringComparison.OrdinalIgnoreCase));
    }

    protected virtual void OnClicked() => Clicked?.Invoke(this, EventArgs.Empty);
    protected virtual void OnShowRequested() => ShowRequested?.Invoke(this, EventArgs.Empty);
    protected virtual void OnExitRequested() => ExitRequested?.Invoke(this, EventArgs.Empty);
}
