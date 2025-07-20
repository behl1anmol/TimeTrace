using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using timetrace.ui.Models;

namespace timetrace.ui.ViewModels;

public partial class PresenterViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<CapturedImageModel> capturedImages = new();

    private readonly int applicationId = 1;
    private readonly IApplicationDataService dataService;

    public int ImageCount => CapturedImages.Count;

    public PresenterViewModel(IApplicationDataService applicationDataService)
    {
        dataService = applicationDataService;
        LoadImages();
    }

    public void ApplyFilter(DateTime? from, DateTime? to, DateTime? specificDay, IList<string> statuses)
    {
        var all = dataService.GetCapturedImages(applicationId);

        var filtered = all.AsQueryable();

        if (from is not null)
            filtered = filtered.Where(x => x.Timestamp >= from.Value);
        if (to is not null)
            filtered = filtered.Where(x => x.Timestamp <= to.Value);
        if (specificDay is not null)
            filtered = filtered.Where(x => x.Timestamp.Date == specificDay.Value.Date);

        if (statuses != null && statuses.Count > 0)
            filtered = filtered.Where(x => statuses.Contains(x.ProcessStatus));

        CapturedImages.Clear();
        foreach (var img in filtered)
            CapturedImages.Add(img);

        OnPropertyChanged(nameof(ImageCount));
    }

    private void LoadImages()
    {
        ApplyFilter(null, null, null, new[] { "Active", "Minimized", "Closed" });
    }
}
