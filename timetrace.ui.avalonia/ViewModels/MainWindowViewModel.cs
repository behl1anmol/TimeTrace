using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using timetrace.ui.avalonia.Services;

namespace timetrace.ui.avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IThemeService? _themeService;
    private readonly IApplicationDataService? _dataService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SidebarWidth))]
    [NotifyPropertyChangedFor(nameof(CollapseIconPath))]
    private bool _isCollapsed;

    [ObservableProperty]
    private object? _currentView;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSettingsSelected))]
    private bool _isApplicationsSelected = true;

    private readonly ApplicationDetailsViewModel _detailsViewModel;

    public bool IsSettingsSelected => !IsApplicationsSelected;

    public double SidebarWidth => IsCollapsed ? 70 : 240;

    // Chevron icons as SVG paths for collapse button
    public string CollapseIconPath => IsCollapsed 
        ? "M8.59 16.59L13.17 12 8.59 7.41 10 6l6 6-6 6-1.41-1.41z"  // ChevronRight
        : "M15.41 16.59L10.83 12l4.58-4.59L14 6l-6 6 6 6 1.41-1.41z"; // ChevronLeft

    public MainWindowViewModel() : this(null, null) { }

    public MainWindowViewModel(IThemeService? themeService, IApplicationDataService? dataService)
    {
        _themeService = themeService;
        _dataService = dataService;
        _detailsViewModel = new ApplicationDetailsViewModel(_dataService);
        
        CurrentView = new ApplicationListViewModel(NavigateToDetails, _dataService);
    }

    [RelayCommand]
    private void NavigateToApplications()
    {
        IsApplicationsSelected = true;
        CurrentView = new ApplicationListViewModel(NavigateToDetails, _dataService);
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        IsApplicationsSelected = false;
        CurrentView = new SettingsViewModel(_themeService);
    }

    [RelayCommand]
    private void ToggleSidebar()
    {
        IsCollapsed = !IsCollapsed;
    }

    private void NavigateToDetails(ApplicationItemViewModel app)
    {
        _detailsViewModel.SetApplication(app);
        CurrentView = _detailsViewModel;
    }
}
