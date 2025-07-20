using System.Windows.Controls;
using timetrace.ui.ViewModels;

namespace timetrace.ui.Views;
/// <summary>
/// Interaction logic for PresenterView.xaml
/// </summary>
public partial class PresenterView : UserControl
{
    public PresenterView(PresenterViewModel presenterViewModel)
    {
        InitializeComponent();
        DataContext = presenterViewModel;
    }
}
