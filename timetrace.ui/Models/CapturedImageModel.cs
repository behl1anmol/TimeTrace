using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace timetrace.ui.Models;

/// <summary>
/// Represents a captured screenshot image.
/// TODO: Map from IImageRepository / timetrace.library.Models.Image entity.
///       Image.ImageId → Id, Image.Name → ScreenshotName, Image.ImagePath → FilePath,
///       Image.DateTimeStamp → Timestamp, Image.ImageGuid → ImageGuid,
///       ProcessDetail.ProcessId → ApplicationId.
/// </summary>
public partial class CapturedImageModel : ObservableObject
{
    // TODO: Maps to Image.ImageId
    [ObservableProperty]
    private int id;

    // TODO: Maps to ProcessDetail.ProcessId (via Image → ProcessDetail FK)
    [ObservableProperty]
    private int applicationId;

    // TODO: Maps to Image.Name
    [ObservableProperty]
    private string screenshotName = string.Empty;

    // TODO: Maps to Image.ImagePath
    [ObservableProperty]
    private string filePath = string.Empty;

    // TODO: Maps to Image.DateTimeStamp
    [ObservableProperty]
    private DateTime timestamp;

    [ObservableProperty]
    private string fileSize = string.Empty;

    [ObservableProperty]
    private string resolution = string.Empty;

    [ObservableProperty]
    private string processStatus = string.Empty;

    // TODO: Maps to Image.ImageGuid
    [ObservableProperty]
    private Guid imageGuid;

    /// <summary>
    /// Loaded image for rendering in the UI.
    /// When FilePath is set, load the BitmapImage from disk.
    /// Falls back to a placeholder when null.
    /// </summary>
    [ObservableProperty]
    private ImageSource? imageSource;

    // Extensible metadata for future features
    public Dictionary<string, object> Metadata { get; set; } = new();
}
