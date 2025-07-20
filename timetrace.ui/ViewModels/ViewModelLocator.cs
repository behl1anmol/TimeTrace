namespace timetrace.ui.ViewModels;
public class ViewModelLocator
{
    public ApplicationListViewModel ApplicationListViewModel =>
        App.ServiceProvider.GetService(typeof(ApplicationListViewModel)) as ApplicationListViewModel;

    public SettingsViewModel SettingsViewModel =>
        App.ServiceProvider.GetService(typeof(SettingsViewModel)) as SettingsViewModel;
}


