using System.ComponentModel;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using timetrace.ui.Controls.Lightbox;

namespace timetrace.ui.Views;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    /// <summary>
    /// Handles the window closing event to minimize to tray instead of closing.
    /// </summary>
    protected override async void OnClosing(CancelEventArgs e)
    {
        // Only handle closing event if we're not in an explicit shutdown
        if (Application.Current.ShutdownMode != ShutdownMode.OnExplicitShutdown)
        {
            // Cancel the close operation for now
            e.Cancel = true;

            // Get the service from DI
            var lightboxService = App.ServiceProvider.GetRequiredService<ILightboxService>();

            // Show exit confirmation lightbox
            var result = await lightboxService.ShowOptionsAsync(
                "Exit Confirmation",
                "Do you want to exit the application or minimize it to the system tray?",
                "Exit Application",
                "Minimize to Tray",
                "Cancel");

            switch (result)
            {
                case LightboxResult.Primary: // Exit
                    Application.Current.Shutdown();
                    break;

                case LightboxResult.Secondary: // Minimize to tray
                    this.WindowState = WindowState.Minimized;
                    this.Hide();
                    break;

                case LightboxResult.Cancel: // Do nothing
                    break;
            }
        }

        base.OnClosing(e);
    }
}
