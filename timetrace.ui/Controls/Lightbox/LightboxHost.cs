using System.Windows;
using System.Windows.Media.Effects;

namespace timetrace.ui.Controls.Lightbox;

public class LightboxHost : ILightboxHost
{
    public async Task<T> ShowLightboxAsync<T>(LightboxViewModel viewModel)
    {
        var owner = Application.Current.MainWindow;
        if (owner == null)
            throw new InvalidOperationException("No main window found for lightbox owner");

        // Store original state
        var originalEffect = owner.Effect;
        var originalIsEnabled = owner.IsEnabled;

        try
        {
            // Apply blur effect and disable the window
            owner.Effect = new BlurEffect { Radius = 5 };
            owner.IsEnabled = false;

            // Create lightbox window
            var lightboxWindow = new Window
            {
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                Topmost = true,
                ShowInTaskbar = false,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = owner,
                Content = new LightboxView { DataContext = viewModel }
            };

            // Show window in a non-blocking way
            lightboxWindow.Show();

            // Wait for result
            var result = await viewModel.Result;

            // Close window
            lightboxWindow.Close();

            return (T)(object)result; // Cast to expected type
        }
        finally
        {
            // Always restore the original state
            owner.Effect = originalEffect;
            owner.IsEnabled = originalIsEnabled;
        }
    }
}
