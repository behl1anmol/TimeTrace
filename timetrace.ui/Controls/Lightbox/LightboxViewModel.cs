using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace timetrace.ui.Controls.Lightbox;

public partial class LightboxViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _message = string.Empty;

    [ObservableProperty]
    private string _primaryButtonText = string.Empty;

    [ObservableProperty]
    private string _secondaryButtonText = string.Empty;

    [ObservableProperty]
    private string _cancelButtonText = string.Empty;

    [ObservableProperty]
    private bool _showSecondaryButton;

    [ObservableProperty]
    private bool _showCancelButton;

    private readonly TaskCompletionSource<LightboxResult> _resultSource = new TaskCompletionSource<LightboxResult>();

    public Task<LightboxResult> Result => _resultSource.Task;

    [RelayCommand]
    private void Primary()
    {
        _resultSource.TrySetResult(LightboxResult.Primary);
    }

    [RelayCommand]
    private void Secondary()
    {
        _resultSource.TrySetResult(LightboxResult.Secondary);
    }

    [RelayCommand]
    private void Cancel()
    {
        _resultSource.TrySetResult(LightboxResult.Cancel);
    }
}
