using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace timetrace.ui.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private bool autoCapture = true;

    [ObservableProperty]
    private int captureIntervalMinutes = 5;

    [ObservableProperty]
    private string screenshotQuality = "High";

    [ObservableProperty]
    private bool monitorAll = true;

    [ObservableProperty]
    private bool includeBackgroundApps = false;

    [ObservableProperty]
    private bool includeSystemApps = false;

    [ObservableProperty]
    private string storagePath = @"C:\TimeTrace\Screenshots";

    [ObservableProperty]
    private int retentionDays = 30;

    [ObservableProperty]
    private bool compressScreenshots = true;

    [ObservableProperty]
    private bool notifyOnCapture = false;

    [ObservableProperty]
    private bool notifyOnNewApp = true;

    public List<string> QualityOptions { get; } = ["Low", "Medium", "High"];

    public SettingsViewModel()
    {
        // TODO: Load initial values from IConfigurationRepository
    }

    [RelayCommand]
    private void SaveSettings()
    {
        // TODO: Persist via IConfigurationRepository.UpdateConfigurationSettingDetail()
    }

    [RelayCommand]
    private void ResetToDefaults()
    {
        AutoCapture = true;
        CaptureIntervalMinutes = 5;
        ScreenshotQuality = "High";
        MonitorAll = true;
        IncludeBackgroundApps = false;
        IncludeSystemApps = false;
        StoragePath = @"C:\TimeTrace\Screenshots";
        RetentionDays = 30;
        CompressScreenshots = true;
        NotifyOnCapture = false;
        NotifyOnNewApp = true;
    }

    [RelayCommand]
    private void BrowseStoragePath()
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select Storage Folder",
            InitialDirectory = StoragePath
        };

        if (dialog.ShowDialog() == true)
        {
            StoragePath = dialog.FolderName;
        }
    }
}
