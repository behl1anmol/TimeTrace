using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using timetrace.ui.avalonia.Services;
using timetrace.ui.avalonia.ViewModels;
using timetrace.ui.avalonia.Views;

namespace timetrace.ui.avalonia;

public partial class App : Application
{
    public static IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Configure DI container
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var themeService = Services.GetRequiredService<IThemeService>();
            var dataService = Services.GetRequiredService<IApplicationDataService>();
            
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(themeService, dataService),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Core services
        services.AddSingleton<IThemeService>(sp => new ThemeService(this));
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IImageLoadingService, ImageLoadingService>();
        
        // Data services - use mock for now, will switch to real implementation when DB is ready
        services.AddSingleton<IApplicationDataService, MockApplicationDataService>();
        
        // ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<ApplicationListViewModel>();
        services.AddTransient<ApplicationDetailsViewModel>();
        services.AddTransient<SettingsViewModel>();
    }
}