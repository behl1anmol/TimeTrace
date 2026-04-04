using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace timetrace.ui.avalonia.ViewModels;

public partial class FilterViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasActiveFilter))]
    [NotifyPropertyChangedFor(nameof(FilterSummary))]
    private DateTime? _fromDate;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasActiveFilter))]
    [NotifyPropertyChangedFor(nameof(FilterSummary))]
    private DateTime? _toDate;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private int _filteredCount;

    public bool HasActiveFilter => FromDate.HasValue || ToDate.HasValue;

    public string FilterSummary
    {
        get
        {
            if (!HasActiveFilter) return string.Empty;
            
            var parts = new List<string>();
            if (FromDate.HasValue) parts.Add($"From: {FromDate:MMM dd, yyyy}");
            if (ToDate.HasValue) parts.Add($"To: {ToDate:MMM dd, yyyy}");
            return string.Join("\n", parts);
        }
    }

    [RelayCommand]
    private void ClearFromDate() => FromDate = null;

    [RelayCommand]
    private void ClearToDate() => ToDate = null;

    [RelayCommand]
    private void ClearAll()
    {
        FromDate = null;
        ToDate = null;
    }
}