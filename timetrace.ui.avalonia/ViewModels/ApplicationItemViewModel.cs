using System.Windows.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace timetrace.ui.avalonia.ViewModels;

public partial class ApplicationItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _processName = string.Empty;

    [ObservableProperty]
    private string _icon = "📱";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRunning))]
    [NotifyPropertyChangedFor(nameof(StatusColor))]
    private string _status = "running";

    [ObservableProperty]
    private int _screenshotCount;

    [ObservableProperty]
    private string _memoryUsage = "0 MB";

    [ObservableProperty] 
    private ICommand? _selectCommand;

    /// <summary>
    /// Returns true if the application status is "running"
    /// </summary>
    public bool IsRunning => Status?.ToLowerInvariant() == "running";

    /// <summary>
    /// Returns the appropriate color for the status indicator
    /// </summary>
    public Color StatusColor => IsRunning 
        ? Color.Parse("#22C55E")  // Green for running
        : Color.Parse("#6B7280"); // Gray for idle
}