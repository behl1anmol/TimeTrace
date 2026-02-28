using CommunityToolkit.Mvvm.ComponentModel;

namespace timetrace.ui.Models;

/// <summary>
/// Represents a monitored application / process.
/// TODO: Map from IProcessRepository.GetProcesses() — aligns with timetrace.library.Models.Process
///       Process.ProcessId → Id, Process.Name → ProcessName
///       CaptureCount is computed from Process → ProcessDetails → Images count.
///       LastCaptured is computed from max Image.DateTimeStamp.
/// </summary>
public partial class ApplicationModel : ObservableObject
{
    // TODO: Maps to Process.ProcessId
    [ObservableProperty]
    private int id;

    // TODO: Maps to Process.Name
    [ObservableProperty]
    private string processName = string.Empty;

    // OS process ID — not stored in library
    [ObservableProperty]
    private int processId;

    // UI-only concept (Active/Minimized/Closed) — not in library
    [ObservableProperty]
    private string status = string.Empty;

    // TODO: Compute from max Image.DateTimeStamp across ProcessDetails
    [ObservableProperty]
    private DateTime lastCaptured;

    // Emoji icon or path — UI-only
    [ObservableProperty]
    private string iconPath = string.Empty;

    // TODO: Compute from count of Images across ProcessDetails
    [ObservableProperty]
    private int captureCount;

    [ObservableProperty]
    private bool isRunning;

    /// <summary>
    /// Total number of captured images for this application.
    /// Alias for CaptureCount — used by detail view header ("N of M screenshots").
    /// </summary>
    public int ImageCount => CaptureCount;

    // Extensible properties dictionary for future enhancements
    public Dictionary<string, object> ExtensionData { get; set; } = new();
}
