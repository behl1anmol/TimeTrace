using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace timetrace.ui.ViewModels;

/// <summary>
/// Manages date-range filtering for the application detail view.
/// Simplified to From/To dates only (no status filter per Figma design).
/// </summary>
public partial class FilterViewModel : ObservableObject
{
    [ObservableProperty]
    private DateTime? fromDate;

    [ObservableProperty]
    private DateTime? toDate;

    /// <summary>
    /// True when any filter is active.
    /// </summary>
    public bool HasActiveFilter => FromDate is not null || ToDate is not null;

    /// <summary>
    /// Formatted summary of active filters for the badge display.
    /// </summary>
    public string FilterSummary
    {
        get
        {
            var parts = new List<string>();
            if (FromDate is not null)
                parts.Add($"From: {FromDate.Value:MMM dd, yyyy}");
            if (ToDate is not null)
                parts.Add($"To: {ToDate.Value:MMM dd, yyyy}");
            return string.Join("  •  ", parts);
        }
    }

    /// <summary>
    /// Event raised when filter values are applied or cleared.
    /// </summary>
    public event EventHandler? FilterChanged;

    partial void OnFromDateChanged(DateTime? value) => NotifyFilterProperties();
    partial void OnToDateChanged(DateTime? value) => NotifyFilterProperties();

    private void NotifyFilterProperties()
    {
        OnPropertyChanged(nameof(HasActiveFilter));
        OnPropertyChanged(nameof(FilterSummary));
    }

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
        ApplyFilter();
    }

    [RelayCommand]
    private void ClearFromDate()
    {
        FromDate = null;
        ApplyFilter();
    }

    [RelayCommand]
    private void ClearToDate()
    {
        ToDate = null;
        ApplyFilter();
    }
}
