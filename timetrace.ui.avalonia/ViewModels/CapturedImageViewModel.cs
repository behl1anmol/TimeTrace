using CommunityToolkit.Mvvm.ComponentModel;
using timetrace.ui.avalonia.Services;

namespace timetrace.ui.avalonia.ViewModels;

public partial class CapturedImageViewModel : ViewModelBase
{
    private readonly IImageLoadingService? _imageService;
    private Avalonia.Media.Imaging.Bitmap? _thumbnail;
    private Avalonia.Media.Imaging.Bitmap? _fullImage;
    private bool _thumbnailLoaded;
    private bool _fullImageLoaded;

    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private DateTime _timestamp;

    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private string _resolution = string.Empty;

    [ObservableProperty]
    private string _fileSize = string.Empty;

    public string FormattedTimestamp => Timestamp.ToString("MMM dd, yyyy HH:mm");

    public Avalonia.Media.Imaging.Bitmap? Thumbnail
    {
        get
        {
            if (!_thumbnailLoaded)
            {
                _thumbnailLoaded = true;
                _ = LoadThumbnailAsync();
            }
            return _thumbnail;
        }
        private set
        {
            if (_thumbnail != value)
            {
                _thumbnail = value;
                OnPropertyChanged();
            }
        }
    }

    public Avalonia.Media.Imaging.Bitmap? FullImage
    {
        get
        {
            if (!_fullImageLoaded)
            {
                _fullImageLoaded = true;
                _ = LoadFullImageAsync();
            }
            return _fullImage;
        }
        private set
        {
            if (_fullImage != value)
            {
                _fullImage = value;
                OnPropertyChanged();
            }
        }
    }

    public CapturedImageViewModel() : this(null) { }

    public CapturedImageViewModel(IImageLoadingService? imageService)
    {
        _imageService = imageService;
    }

    private async Task LoadThumbnailAsync()
    {
        if (_imageService != null && !string.IsNullOrEmpty(FilePath))
        {
            Thumbnail = await _imageService.LoadThumbnailAsync(FilePath, 200);
        }
    }

    private async Task LoadFullImageAsync()
    {
        if (_imageService != null && !string.IsNullOrEmpty(FilePath))
        {
            FullImage = await _imageService.LoadImageAsync(FilePath);
        }
    }
}