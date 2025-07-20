using timetrace.ui.ViewModels;

namespace timetrace.ui.Helpers;
public class ViewModelLocator
{
    public ApplicationListViewModel ApplicationListViewModel =>
        (ApplicationListViewModel)App.ServiceProvider.GetService(typeof(ApplicationListViewModel))!;

    public SettingsViewModel SettingsViewModel =>
        (SettingsViewModel)App.ServiceProvider.GetService(typeof(SettingsViewModel))!;

    public FilterViewModel FilterViewModel =>
        (FilterViewModel)App.ServiceProvider.GetService(typeof(FilterViewModel))!;

    public PresenterViewModel PresenterViewModel =>
        (PresenterViewModel)App.ServiceProvider.GetService(typeof(PresenterViewModel))!;

    public ApplicationDetailsViewModel ApplicationDetailsViewModel =>
        (ApplicationDetailsViewModel)App.ServiceProvider.GetService(typeof(ApplicationDetailsViewModel))!;

    public MainViewModel MainViewModel =>
        (MainViewModel)App.ServiceProvider.GetService(typeof(MainViewModel))!;
}


