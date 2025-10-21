namespace timetrace.ui.Services.Tray;

/// <summary>
/// Defines the interface for a system tray icon service.
/// </summary>
public interface ITrayIconService : IDisposable
{
    /// <summary>
    /// Initializes the tray icon and sets up event handlers.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Displays a notification in the system tray.
    /// </summary>
    /// <param name="title">The title of the notification.</param>
    /// <param name="message">The message content of the notification.</param>
    void ShowNotification(string title, string message);
}