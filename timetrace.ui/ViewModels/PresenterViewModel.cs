using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using timetrace.ui.Models;

namespace timetrace.ui.ViewModels;

/// <summary>
/// Thin ViewModel kept for DataTemplate resolution.
/// Image data is owned by ApplicationDetailsViewModel and bound from the parent view.
/// </summary>
public partial class PresenterViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<CapturedImageModel> capturedImages = [];

    public int ImageCount => CapturedImages.Count;

    public PresenterViewModel()
    {
    }
}
