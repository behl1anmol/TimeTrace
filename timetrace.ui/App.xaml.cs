using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using timetrace.ui.Models;
using timetrace.ui.Services;
using timetrace.ui.ViewModels;
using timetrace.ui.Views;

namespace timetrace.ui;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static ServiceProvider ServiceProvider
    {
        get; private set;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();

        // Register services
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IApplicationDataService, ApplicationDataService>();

        // Register ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<ApplicationListViewModel>();
        services.AddSingleton<FilterViewModel>();
        services.AddSingleton<PresenterViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<ApplicationDetailsViewModel>();

        services.AddSingleton<ApplicationModel>();
        services.AddSingleton<CapturedImageModel>();

        ServiceProvider = services.BuildServiceProvider();

        // Retrieve the MainViewModel instance from the ServiceProvider
        var mainViewModel = ServiceProvider.GetService<MainViewModel>();

        // Pass the MainViewModel instance to the MainWindow constructor
        var mainWindow = new MainWindow(mainViewModel)
        {
            DataContext = mainViewModel
        };
        mainWindow.Show();

        base.OnStartup(e);
    }
}

