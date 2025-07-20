using CommunityToolkit.Mvvm.ComponentModel;
using timetrace.ui.Models;
using timetrace.ui.Services;

namespace timetrace.ui.ViewModels;

public partial class ApplicationDetailsViewModel : ObservableObject
{
    // Sub-viewmodels for left and right sides
    [ObservableProperty]
    private FilterViewModel filterViewModel;

    [ObservableProperty]
    private PresenterViewModel presenterViewModel;

    [ObservableProperty]
    private ApplicationModel application;

    public ApplicationDetailsViewModel(
        ApplicationModel applicationModel,
        INavigationService navigationService,
        FilterViewModel filterViewModel,
        PresenterViewModel presenterViewModel)
    {
        Application = applicationModel;
        // Pass reference or events as needed for full MVVM filtering
        FilterViewModel = filterViewModel;
        PresenterViewModel = presenterViewModel;

        // Example mechanism: listen for filter changes and update gallery
        FilterViewModel.FilterChanged += (sender, args) =>
        {
            PresenterViewModel.ApplyFilter(
                FilterViewModel.FromDate,
                FilterViewModel.ToDate,
                FilterViewModel.SelectedDate,
                FilterViewModel.Statuses);
        };
    }
}
