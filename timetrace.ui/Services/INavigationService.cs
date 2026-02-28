namespace timetrace.ui.Services;

public interface INavigationService
{
    event Action<object, string?>? Navigated;
    void NavigateTo(object viewModel);
    void NavigateToDetails(object detailsViewModel);
    void NavigateBack();
}
