using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using timetrace.ui.Models;
using timetrace.ui.Services;

namespace timetrace.ui.ViewModels;

/// <summary>
/// ViewModel for the application detail screen — owns header state,
/// filter wiring, image collection, carousel navigation, and view-mode toggle.
/// </summary>
public partial class ApplicationDetailsViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IApplicationDataService _dataService;

    // Sub-viewmodels
    [ObservableProperty]
    private FilterViewModel filterViewModel;

    [ObservableProperty]
    private ApplicationModel application = new();

    // ---------- Image collections ----------

    /// <summary>All images loaded for the current application (unfiltered).</summary>
    private List<CapturedImageModel> _allImages = [];

    /// <summary>Images after date-range filter has been applied.</summary>
    [ObservableProperty]
    private ObservableCollection<CapturedImageModel> filteredImages = [];

    // ---------- View mode ----------

    /// <summary>"Carousel" or "Grid".</summary>
    [ObservableProperty]
    private string viewMode = "Carousel";

    public bool IsCarouselMode => ViewMode == "Carousel";
    public bool IsGridMode => ViewMode == "Grid";

    partial void OnViewModeChanged(string value)
    {
        OnPropertyChanged(nameof(IsCarouselMode));
        OnPropertyChanged(nameof(IsGridMode));
    }

    // ---------- Counts ----------

    public int TotalImageCount => _allImages.Count;
    public int FilteredImageCount => FilteredImages.Count;

    // ---------- Carousel state ----------

    [ObservableProperty]
    private int currentIndex;

    public CapturedImageModel? CurrentImage =>
        FilteredImages.Count > 0 && CurrentIndex >= 0 && CurrentIndex < FilteredImages.Count
            ? FilteredImages[CurrentIndex]
            : null;

    /// <summary>Human-readable "1 / N" counter.</summary>
    public string CarouselCounter =>
        FilteredImages.Count > 0
            ? $"{CurrentIndex + 1} / {FilteredImages.Count}"
            : "0 / 0";

    partial void OnCurrentIndexChanged(int value)
    {
        OnPropertyChanged(nameof(CurrentImage));
        OnPropertyChanged(nameof(CarouselCounter));
    }

    // ---------- Constructor ----------

    public ApplicationDetailsViewModel(
        INavigationService navigationService,
        IApplicationDataService applicationDataService,
        FilterViewModel filterViewModel)
    {
        _navigationService = navigationService;
        _dataService = applicationDataService;
        FilterViewModel = filterViewModel;

        FilterViewModel.FilterChanged += (_, _) => ApplyDateFilter();
    }

    // ---------- Public API ----------

    /// <summary>
    /// Called by ApplicationListViewModel before navigating here.
    /// Loads all images for the selected application.
    /// </summary>
    public void SetApplication(ApplicationModel app)
    {
        Application = app;
        _allImages = _dataService.GetCapturedImagesForProcess(app.Id);

        // Reset filter state for fresh view
        FilterViewModel.ClearFilterCommand.Execute(null);
        // ClearFilter already fires FilterChanged → ApplyDateFilter
    }

    // ---------- Commands ----------

    [RelayCommand]
    private void NavigateBack()
    {
        _navigationService.NavigateBack();
    }

    [RelayCommand]
    private void SwitchToCarousel()
    {
        ViewMode = "Carousel";
    }

    [RelayCommand]
    private void SwitchToGrid()
    {
        ViewMode = "Grid";
    }

    [RelayCommand]
    private void NextImage()
    {
        if (CurrentIndex < FilteredImages.Count - 1)
            CurrentIndex++;
    }

    [RelayCommand]
    private void PreviousImage()
    {
        if (CurrentIndex > 0)
            CurrentIndex--;
    }

    [RelayCommand]
    private void GoToImage(int index)
    {
        if (index >= 0 && index < FilteredImages.Count)
        {
            CurrentIndex = index;
            ViewMode = "Carousel";
        }
    }

    // ---------- Filtering ----------

    private void ApplyDateFilter()
    {
        var from = FilterViewModel.FromDate;
        var to = FilterViewModel.ToDate;

        IEnumerable<CapturedImageModel> result = _allImages;

        if (from is not null)
            result = result.Where(img => img.Timestamp >= from.Value);
        if (to is not null)
            result = result.Where(img => img.Timestamp <= to.Value.Date.AddDays(1));

        FilteredImages = new ObservableCollection<CapturedImageModel>(result);

        // Reset carousel to first image
        CurrentIndex = 0;

        OnPropertyChanged(nameof(TotalImageCount));
        OnPropertyChanged(nameof(FilteredImageCount));
        OnPropertyChanged(nameof(CurrentImage));
        OnPropertyChanged(nameof(CarouselCounter));
    }
}
