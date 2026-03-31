using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace timetrace.ui.avalonia.Views;

public partial class ApplicationDetailsView : UserControl
{
    public ApplicationDetailsView()
    {
        InitializeComponent();
        
        // Wire up back button to MainViewModel
        var backButton = this.FindControl<Button>("BackButton");
        if (backButton != null)
        {
            backButton.Click += OnBackButtonClick;
        }
    }

    private void OnBackButtonClick(object? sender, RoutedEventArgs e)
    {
        // Find the MainWindow and navigate back
        var window = this.GetVisualRoot() as MainWindow;
        if (window?.DataContext is ViewModels.MainWindowViewModel mainVm)
        {
            mainVm.NavigateToApplicationsCommand.Execute(null);
        }
    }
}
