using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using timetrace.ui.avalonia.Services;

namespace timetrace.ui.avalonia.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly IThemeService? _themeService;

    // Capture Settings
    [ObservableProperty]
    private bool _autoCaptureEnabled = true;

    [ObservableProperty]
    private int _captureIntervalSeconds = 30;

    [ObservableProperty]
    private string _screenshotQuality = "High";

    // Monitoring Settings
    [ObservableProperty]
    private bool _monitorAllApps = true;

    [ObservableProperty]
    private bool _includeBackgroundApps;

    [ObservableProperty]
    private bool _includeSystemApps;

    // Storage Settings
    [ObservableProperty]
    private string _storagePath = string.Empty;

    [ObservableProperty]
    private int _retentionDays = 30;

    [ObservableProperty]
    private bool _compressScreenshots = true;

    [ObservableProperty]
    private int _maxStorageSizeMB = 500;

    // Notifications
    [ObservableProperty]
    private bool _notifyOnCapture;

    [ObservableProperty]
    private bool _notifyOnNewApp = true;

    // Theme
    [ObservableProperty]
    private bool _isDarkMode = true;

    public string[] QualityOptions { get; } = ["Low", "Medium", "High"];

    public SettingsViewModel() : this(null) { }

    public SettingsViewModel(IThemeService? themeService)
    {
        _themeService = themeService;
        
        // Set default storage path
        StoragePath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "timetrace", "captures");

        // Initialize theme state from current application theme
        if (_themeService != null)
        {
            _isDarkMode = _themeService.IsDarkMode;
        }
    }

    [RelayCommand]
    private void SaveSettings()
    {
        // TODO: Persist to IConfigurationRepository
    }

    [RelayCommand]
    private void ResetToDefaults()
    {
        AutoCaptureEnabled = true;
        CaptureIntervalSeconds = 30;
        ScreenshotQuality = "High";
        MonitorAllApps = true;
        IncludeBackgroundApps = false;
        IncludeSystemApps = false;
        RetentionDays = 30;
        CompressScreenshots = true;
        MaxStorageSizeMB = 500;
        NotifyOnCapture = false;
        NotifyOnNewApp = true;
        IsDarkMode = true;
    }

    [RelayCommand]
    private async Task BrowseStoragePath()
    {
        // TODO: Open folder picker dialog via IStorageProvider
        await Task.CompletedTask;
    }

    partial void OnIsDarkModeChanged(bool value)
    {
        _themeService?.SetTheme(value ? ThemeVariant.Dark : ThemeVariant.Light);
    }
}
