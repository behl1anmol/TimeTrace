using System.Diagnostics;
using System.Windows;

namespace timetrace.ui.Helpers;

public static class ViewLocator
{
    // Ideally inject this or get it from your DI container
    private static readonly ViewModelLocator _viewModelLocator = new();

    private const string _propertyName = "ViewModel";

    // Map keys to view model instances or retrieval functions
    private static readonly IReadOnlyDictionary<string, object> _viewModelMap =
        new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            { nameof(ViewModelLocator.ApplicationListViewModel), _viewModelLocator.ApplicationListViewModel },
            { nameof(ViewModelLocator.SettingsViewModel), _viewModelLocator.SettingsViewModel },
            { nameof(ViewModelLocator.FilterViewModel), _viewModelLocator.FilterViewModel },
            { nameof(ViewModelLocator.PresenterViewModel), _viewModelLocator.PresenterViewModel },
            { nameof(ViewModelLocator.ApplicationDetailsViewModel), _viewModelLocator.ApplicationDetailsViewModel },
            { nameof(ViewModelLocator.MainViewModel), _viewModelLocator.MainViewModel }
        };

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.RegisterAttached(
        _propertyName,
        typeof(string),
        typeof(ViewLocator),
        new PropertyMetadata(null, OnViewModelChanged));

    public static void SetViewModel(DependencyObject element, string value) =>
        element.SetValue(ViewModelProperty, value);

    public static string GetViewModel(DependencyObject element) =>
        (string)element.GetValue(ViewModelProperty);

    private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement view && e.NewValue is string viewModelKey)
        {
            if (_viewModelMap.TryGetValue(viewModelKey, out var viewModel))
            {
                view.DataContext = viewModel;
            }
            else
            {
                view.DataContext = null; // Clear if not found
                Debug.WriteLine($"ViewLocator: ViewModel '{viewModelKey}' not found.");
            }
        }
    }
}
