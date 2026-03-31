using Avalonia.Controls;
using Avalonia.Input;

namespace timetrace.ui.avalonia.Views;

public partial class CarouselView : UserControl
{
    public CarouselView()
    {
        InitializeComponent();
        
        // Enable keyboard navigation
        Focusable = true;
        KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is ViewModels.ApplicationDetailsViewModel vm)
        {
            switch (e.Key)
            {
                case Key.Left:
                    vm.PreviousImageCommand.Execute(null);
                    e.Handled = true;
                    break;
                case Key.Right:
                    vm.NextImageCommand.Execute(null);
                    e.Handled = true;
                    break;
            }
        }
    }
}
