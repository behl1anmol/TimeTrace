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

    public MainViewModel(ApplicationListViewModel applicationListViewModel, SettingsViewModel settingsViewModel, INavigationService navigationService)
    {
        _applicationListViewModel = applicationListViewModel;
        _settingsViewModel = settingsViewModel;
        CurrentView = applicationListViewModel;
        navigationService.Navigated += (viewModel, title) =>
        {
            CurrentView = viewModel;
            CurrentViewTitle = title ?? string.Empty;
        };
    }

    [RelayCommand]
    private void NavigateToApplications()
    {
        CurrentView = _applicationListViewModel;
        CurrentViewTitle = "Applications";
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        CurrentView = _settingsViewModel;
        CurrentViewTitle = "Settings";
    }

    [RelayCommand]
    private void ToggleSidebar()
    {
        IsCollapsed = !IsCollapsed;
    }
}
