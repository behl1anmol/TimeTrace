using System.Windows;
using System.Windows.Controls;
using timetrace.ui.ViewModels;

namespace timetrace.ui.Views;

public class ViewLocator
{
    private static readonly ViewModelLocator s_viewModelLocator = new();

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.RegisterAttached("ViewModel", typeof(string),
        typeof(ViewModelLocator), new PropertyMetadata(new PropertyChangedCallback(OnChanged)));

    public static void SetViewModel(UserControl view, string value) => view.SetValue(ViewModelProperty, value);

    public static string GetViewModel(UserControl view) => (string)view.GetValue(ViewModelProperty);

    private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        UserControl view = (UserControl)d;
        string viewModel = e.NewValue as string;
        switch (viewModel)
        {
            case "ApplicationListViewModel":
                view.DataContext = s_viewModelLocator.ApplicationListViewModel;
                break;
            case "SettingsViewModel":
                view.DataContext = s_viewModelLocator.SettingsViewModel;
                break;
            default:
                view.DataContext = null;
                break;
        }
    }
}
