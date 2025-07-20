using CommunityToolkit.Mvvm.ComponentModel;

namespace timetrace.ui.Models;

public partial class ApplicationModel : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string processName = string.Empty;

    [ObservableProperty]
    private int processId;

    [ObservableProperty]
    private string status = string.Empty;

    [ObservableProperty]
    private DateTime lastCaptured;

    [ObservableProperty]
    private string iconPath = string.Empty;

    [ObservableProperty]
    private int captureCount;

    [ObservableProperty]
    private bool isRunning;

    // Extensible properties dictionary for future enhancements
    public Dictionary<string, object> ExtensionData { get; set; } = new();
}
