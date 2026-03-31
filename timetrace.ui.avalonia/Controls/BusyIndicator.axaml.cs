using Avalonia;
using Avalonia.Controls;

namespace timetrace.ui.avalonia.Controls;

public partial class BusyIndicator : UserControl
{
    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<BusyIndicator, bool>(nameof(IsActive));

    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<BusyIndicator, string>(nameof(Message), "Loading...");

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public BusyIndicator()
    {
        InitializeComponent();
    }
}
