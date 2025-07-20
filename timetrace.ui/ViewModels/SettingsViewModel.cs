using CommunityToolkit.Mvvm.ComponentModel;

namespace timetrace.ui.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private bool autoStart = false;

    [ObservableProperty]
    private string screenshotsPath = @"C:\AppMonitor\Screenshots";

    [ObservableProperty]
    private bool notifyOnNewCapture = true;

    // Add RelayCommands for saving or resetting settings as needed

    public SettingsViewModel()
    {
        // Initialize any additional settings or commands here
        // For example, you could load settings from a configuration file or database
    }
}
