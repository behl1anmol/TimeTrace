using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using timetrace.ui.Models;
using timetrace.ui.Services;
using NavigationService = timetrace.ui.Services.NavigationService;

namespace timetrace.ui.ViewModels;

public partial class ApplicationListViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IApplicationDataService _dataService;
    private readonly ApplicationDetailsViewModel _applicationDetailsViewModel;

    [ObservableProperty]
    private ObservableCollection<ApplicationModel> applications = new();

    [ObservableProperty]
    private ApplicationModel? selectedApplication;

    public ApplicationListViewModel(INavigationService navigationService, ApplicationDetailsViewModel applicationDetailsViewModel, IApplicationDataService applicationDataService)
    {
        _navigationService = navigationService;
        _dataService = applicationDataService;
        LoadApplications();
        _applicationDetailsViewModel = applicationDetailsViewModel;

    }

    private void LoadApplications()
    {
        var apps = _dataService.GetApplications();
        Applications.Clear();
        foreach (var app in apps)
        {
            Applications.Add(app);
        }
    }

    [RelayCommand]
    private void SelectApplication(ApplicationModel application)
    {
        SelectedApplication = application;
        _applicationDetailsViewModel.SetApplication(application);

        if (_navigationService is NavigationService navService)
        {
            navService.NavigateToDetails(_applicationDetailsViewModel);
        }
    }

    [RelayCommand]
    private void RefreshApplications()
    {
        LoadApplications();
    }
}
