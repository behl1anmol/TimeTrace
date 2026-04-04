using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using timetrace.ui.avalonia.Services;

namespace timetrace.ui.avalonia.ViewModels;

public partial class ApplicationDetailsViewModel : ViewModelBase
{
    private readonly IApplicationDataService? _dataService;

    [ObservableProperty]
    private ApplicationItemViewModel? _application;

    [ObservableProperty]
    private ObservableCollection<CapturedImageViewModel> _allImages = new();

    [ObservableProperty]
    private ObservableCollection<CapturedImageViewModel> _filteredImages = new();

    [ObservableProperty]
    private string _viewMode = "Carousel"; // "Carousel" or "Grid"

    [ObservableProperty]
    private int _currentIndex;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private FilterViewModel _filter = new();

    public bool IsCarouselMode => ViewMode == "Carousel";
    public bool IsGridMode => ViewMode == "Grid";
    public bool IsEmpty => FilteredImages.Count == 0 && !IsLoading;

    public CapturedImageViewModel? CurrentImage => 
        FilteredImages.Count > 0 && CurrentIndex >= 0 && CurrentIndex < FilteredImages.Count 
            ? FilteredImages[CurrentIndex] 
            : null;

    public string ImageCounter => FilteredImages.Count > 0 
        ? $"{CurrentIndex + 1} / {FilteredImages.Count}" 
        : "0 / 0";

    public ApplicationDetailsViewModel() : this(null)
    {
    }

    public ApplicationDetailsViewModel(IApplicationDataService? dataService)
    {
        _dataService = dataService;
        Filter.PropertyChanged += (s, e) => ApplyFilter();
    }

    public void SetApplication(ApplicationItemViewModel app)
    {
        Application = app;
        _ = LoadImagesAsync();
    }

    private async Task LoadImagesAsync()
    {
        if (Application == null) return;

        if (_dataService == null)
        {
            // Fallback to mock data
            LoadMockImages();
            ApplyFilter();
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var images = await _dataService.GetImagesForProcessAsync(Application.Id);
            
            AllImages.Clear();
            foreach (var img in images)
            {
                AllImages.Add(img);
            }

            ApplyFilter();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load images: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(IsEmpty));
        }
    }

    private void LoadMockImages()
    {
        AllImages.Clear();
        var baseDate = DateTime.Now.AddDays(-7);
        
        for (int i = 0; i < 25; i++)
        {
            AllImages.Add(new CapturedImageViewModel
            {
                Id = i + 1,
                FileName = $"Screenshot_{i + 1:D3}.png",
                Timestamp = baseDate.AddHours(i * 3),
                FilePath = $"/captures/{Application?.ProcessName}/{i + 1:D3}.png",
                Resolution = "1920x1080",
                FileSize = $"{(1.2 + i * 0.1):F1} MB"
            });
        }
    }

    private void ApplyFilter()
    {
        FilteredImages.Clear();
        
        foreach (var img in AllImages)
        {
            bool include = true;
            
            if (Filter.FromDate.HasValue && img.Timestamp < Filter.FromDate.Value)
                include = false;
            
            if (Filter.ToDate.HasValue && img.Timestamp > Filter.ToDate.Value.AddDays(1))
                include = false;
            
            if (include)
                FilteredImages.Add(img);
        }
        
        CurrentIndex = 0;
        OnPropertyChanged(nameof(CurrentImage));
        OnPropertyChanged(nameof(ImageCounter));
        OnPropertyChanged(nameof(IsEmpty));
    }

    [RelayCommand]
    private void SwitchToCarousel()
    {
        ViewMode = "Carousel";
        OnPropertyChanged(nameof(IsCarouselMode));
        OnPropertyChanged(nameof(IsGridMode));
    }

    [RelayCommand]
    private void SwitchToGrid()
    {
        ViewMode = "Grid";
        OnPropertyChanged(nameof(IsCarouselMode));
        OnPropertyChanged(nameof(IsGridMode));
    }

    [RelayCommand]
    private void NextImage()
    {
        if (CurrentIndex < FilteredImages.Count - 1)
        {
            CurrentIndex++;
            OnPropertyChanged(nameof(CurrentImage));
            OnPropertyChanged(nameof(ImageCounter));
        }
    }

    [RelayCommand]
    private void PreviousImage()
    {
        if (CurrentIndex > 0)
        {
            CurrentIndex--;
            OnPropertyChanged(nameof(CurrentImage));
            OnPropertyChanged(nameof(ImageCounter));
        }
    }

    [RelayCommand]
    private void GoToImage(int index)
    {
        if (index >= 0 && index < FilteredImages.Count)
        {
            CurrentIndex = index;
            OnPropertyChanged(nameof(CurrentImage));
            OnPropertyChanged(nameof(ImageCounter));
        }
    }

    [RelayCommand]
    private void SelectImageFromGrid(CapturedImageViewModel image)
    {
        var index = FilteredImages.IndexOf(image);
        if (index >= 0)
        {
            CurrentIndex = index;
            ViewMode = "Carousel";
            OnPropertyChanged(nameof(IsCarouselMode));
            OnPropertyChanged(nameof(IsGridMode));
            OnPropertyChanged(nameof(CurrentImage));
            OnPropertyChanged(nameof(ImageCounter));
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadImagesAsync();
    }
}