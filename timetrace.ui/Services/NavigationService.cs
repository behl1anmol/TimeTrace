using timetrace.ui.ViewModels;

namespace timetrace.ui.Services;

public class NavigationService : INavigationService
{
    private readonly MainViewModel _mainViewModel;

    public event Action<object, string?>? Navigated;

    public void NavigateTo(object viewModel)
    {
        Navigated?.Invoke(viewModel, null);
    }

    public void NavigateToDetails(object detailsViewModel)
    {
        Navigated?.Invoke(detailsViewModel, "Details");
    }

    public void NavigateBack()
    {
        Navigated?.Invoke(null!, "Back");
    }
}
