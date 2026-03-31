namespace timetrace.ui.avalonia.Services;

/// <summary>
/// Service for navigating between views
/// </summary>
public interface INavigationService
{
    event Action<object, string?>? Navigated;
    void NavigateTo(object viewModel, string? title = null);
    void NavigateBack();
}

public class NavigationService : INavigationService
{
    public event Action<object, string?>? Navigated;

    public void NavigateTo(object viewModel, string? title = null)
    {
        Navigated?.Invoke(viewModel, title);
    }

    public void NavigateBack()
    {
        // Implementation depends on navigation stack - simplified for now
        Navigated?.Invoke(null!, "Back");
    }
}
