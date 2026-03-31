using Avalonia;
using Avalonia.Styling;
using Avalonia.Threading;

namespace timetrace.ui.avalonia.Services;

/// <summary>
/// Service for managing application theme (Dark/Light mode)
/// Supports system theme detection for auto light/dark mode
/// </summary>
public interface IThemeService
{
    ThemeVariant CurrentTheme { get; }
    bool IsDarkMode { get; }
    void SetTheme(ThemeVariant theme);
    void ToggleTheme();
    void UseSystemTheme();
}

public class ThemeService : IThemeService
{
    private readonly Application _app;

    public ThemeService(Application app)
    {
        _app = app;
    }

    public ThemeVariant CurrentTheme
    {
        get
        {
            var requested = _app.RequestedThemeVariant;
            
            // If using default/system, detect actual theme
            if (requested == ThemeVariant.Default || requested == null)
            {
                return _app.ActualThemeVariant ?? ThemeVariant.Light;
            }
            
            return requested;
        }
    }

    public bool IsDarkMode => CurrentTheme == ThemeVariant.Dark;

    public void SetTheme(ThemeVariant theme)
    {
        Dispatcher.UIThread.Post(() =>
        {
            _app.RequestedThemeVariant = theme;
        });
    }

    public void ToggleTheme()
    {
        SetTheme(IsDarkMode ? ThemeVariant.Light : ThemeVariant.Dark);
    }

    /// <summary>
    /// Set theme to follow system preference (auto dark/light)
    /// </summary>
    public void UseSystemTheme()
    {
        SetTheme(ThemeVariant.Default);
    }
}
