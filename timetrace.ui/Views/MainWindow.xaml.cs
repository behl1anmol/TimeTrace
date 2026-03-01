using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Extensions.DependencyInjection;
using timetrace.ui.Controls.Lightbox;

namespace timetrace.ui.Views;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private static readonly Duration PageTransitionDuration = new(TimeSpan.FromMilliseconds(200));
    private static readonly CubicEase PageEase = new() { EasingMode = EasingMode.EaseOut };

    public MainWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is INotifyPropertyChanged oldVm)
            oldVm.PropertyChanged -= OnMainVmPropertyChanged;
        if (e.NewValue is INotifyPropertyChanged newVm)
            newVm.PropertyChanged += OnMainVmPropertyChanged;
    }

    private void OnMainVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != "CurrentView") return;
        AnimatePageTransition();
    }

    private void AnimatePageTransition()
    {
        // Slide up from 30px + fade in — matches Windows 11 page entrance
        var fadeIn = new DoubleAnimation(0, 1, PageTransitionDuration) { EasingFunction = PageEase };
        var slideUp = new DoubleAnimation(30, 0, PageTransitionDuration) { EasingFunction = PageEase };

        MainContent.BeginAnimation(OpacityProperty, fadeIn);
        ContentTranslate.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, slideUp);
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
