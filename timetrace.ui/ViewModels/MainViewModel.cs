using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using timetrace.ui.Services;

namespace timetrace.ui.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ApplicationListViewModel _applicationListViewModel;
    private readonly SettingsViewModel _settingsViewModel;

    [ObservableProperty]
    private object? currentView;

    [ObservableProperty]
    private string currentViewTitle = "Applications";

    [ObservableProperty]
    private bool isCollapsed = false;

    [ObservableProperty]
    private string activeNavItem = "Applications";

    public MainViewModel(ApplicationListViewModel applicationListViewModel, SettingsViewModel settingsViewModel, INavigationService navigationService)
    {
        _applicationListViewModel = applicationListViewModel;
        _settingsViewModel = settingsViewModel;
        CurrentView = applicationListViewModel;
        navigationService.Navigated += (viewModel, title) =>
        {
            if (title == "Back")
            {
                CurrentView = _applicationListViewModel;
                CurrentViewTitle = "Applications";
                ActiveNavItem = "Applications";
                return;
            }

            CurrentView = viewModel;
            CurrentViewTitle = title ?? string.Empty;

            if (title is "Details" or null && viewModel is ApplicationDetailsViewModel)
                ActiveNavItem = "Applications";
        };
    }

    [RelayCommand]
    private void NavigateToApplications()
    {
        CurrentView = _applicationListViewModel;
        CurrentViewTitle = "Applications";
        ActiveNavItem = "Applications";
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        CurrentView = _settingsViewModel;
        CurrentViewTitle = "Settings";
        ActiveNavItem = "Settings";
    }

    [RelayCommand]
    private void ToggleSidebar()
    {
        IsCollapsed = !IsCollapsed;
    }
}
