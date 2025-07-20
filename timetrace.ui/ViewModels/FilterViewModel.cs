using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace timetrace.ui.ViewModels;

public partial class FilterViewModel : ObservableObject
{
    [ObservableProperty]
    private DateTime? fromDate;

    [ObservableProperty]
    private DateTime? toDate;

    [ObservableProperty]
    private DateTime? selectedDate;

    [ObservableProperty]
    private bool showActive = true;

    [ObservableProperty]
    private bool showMinimized = true;

    [ObservableProperty]
    private bool showClosed = true;

    public IList<string> Statuses
    {
        get
        {
            var statuses = new List<string>();
            if (ShowActive) statuses.Add("Active");
            if (ShowMinimized) statuses.Add("Minimized");
            if (ShowClosed) statuses.Add("Closed");
            return statuses;
        }
    }

    // Event raised when filter values change
    public event EventHandler? FilterChanged;

    [RelayCommand]
    private void ApplyFilter()
    {
        FilterChanged?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void ClearFilter()
    {
        FromDate = null;
        ToDate = null;
        SelectedDate = null;
        ShowActive = true;
        ShowMinimized = true;
        ShowClosed = true;
        ApplyFilter();
    }
}
