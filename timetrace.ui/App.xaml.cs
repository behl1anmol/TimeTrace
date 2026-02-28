using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using timetrace.ui.Controls.Lightbox;
using timetrace.ui.Models;
using timetrace.ui.Services;
using timetrace.ui.Services.Tray;
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
        ConfigureServices();

        base.OnStartup(e);

        // Instantiate MainWindow and assign it to this.MainWindow
        this.MainWindow = new timetrace.ui.Views.MainWindow();
        // Initialize the tray icon service after the main window is loaded
        this.MainWindow.Loaded += MainWindow_Loaded;
        this.MainWindow.Show();
    }

    private static void ConfigureServices()
    {
        var services = new ServiceCollection();

        // Register services
        // TODO: Replace ApplicationDataService with real implementation using timetrace.library repositories
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IApplicationDataService, ApplicationDataService>();
        services.AddSingleton<ITrayIconService, TrayIconService>();
        services.AddSingleton<ILightboxHost, LightboxHost>();
        services.AddSingleton<ILightboxService, LightboxService>();

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

        //Retrieve the MainViewModel instance from the ServiceProvider

        //var mainViewModel = ServiceProvider.GetService<MainViewModel>();

        // Pass the MainViewModel instance to the MainWindow constructor
        //var mainWindow = new MainWindow(mainViewModel)
        //{
        //    DataContext = mainViewModel
        //};
        //mainWindow.Show();
    }

    /// <summary>
    /// Initializes services that require the main window to be fully loaded.
    /// </summary>
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Initialize the tray icon after the main window is loaded
        Dispatcher.BeginInvoke(new Action(() => {
            if (ServiceProvider != null)
            {
                var trayIconService = ServiceProvider.GetRequiredService<ITrayIconService>();
                trayIconService.Initialize();
            }
        }));

        // Remove the event handler to prevent multiple initializations
        this.MainWindow.Loaded -= MainWindow_Loaded;
    }

    /// <summary>
    /// Cleans up resources when the application exits.
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        // Clean up tray icon when application exits
        if (ServiceProvider != null)
        {
            var trayIconService = ServiceProvider.GetService<ITrayIconService>();
            trayIconService?.Dispose();
        }

        base.OnExit(e);
    }
}

