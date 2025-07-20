using System.Windows.Controls;
using timetrace.ui.ViewModels;

namespace timetrace.ui.Views;
/// <summary>
/// Interaction logic for FilterView.xaml
/// </summary>
public partial class FilterView : UserControl
{
    public FilterView(FilterViewModel filterViewModel)
    {
        InitializeComponent();
        DataContext = filterViewModel;
    }
}
