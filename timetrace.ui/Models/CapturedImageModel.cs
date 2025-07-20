using CommunityToolkit.Mvvm.ComponentModel;

namespace timetrace.ui.Models;

public partial class CapturedImageModel : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private int applicationId;

    [ObservableProperty]
    private string screenshotName = string.Empty;

    [ObservableProperty]
    private string filePath = string.Empty;

    [ObservableProperty]
    private DateTime timestamp;

    [ObservableProperty]
    private string fileSize = string.Empty;

    [ObservableProperty]
    private string resolution = string.Empty;

    [ObservableProperty]
    private string processStatus = string.Empty;

    // Extensible metadata for future features
    public Dictionary<string, object> Metadata { get; set; } = new();
}
