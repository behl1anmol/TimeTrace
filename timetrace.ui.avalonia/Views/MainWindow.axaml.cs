using Avalonia.Controls;
using Avalonia.Input;
using timetrace.ui.avalonia.ViewModels;

namespace timetrace.ui.avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    // If in details view, go back to applications list
                    if (vm.CurrentView is ApplicationDetailsViewModel)
                    {
                        vm.NavigateToApplicationsCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;
                    
                case Key.F5:
                    // Refresh shortcut
                    if (vm.CurrentView is ApplicationListViewModel listVm)
                    {
                        listVm.RefreshCommand.Execute(null);
                        e.Handled = true;
                    }
                    else if (vm.CurrentView is ApplicationDetailsViewModel detailsVm)
                    {
                        detailsVm.RefreshCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;
            }
        }
    }
}