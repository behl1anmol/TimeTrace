using System.Windows.Controls;
using timetrace.ui.ViewModels;

namespace timetrace.ui.Views;
/// <summary>
/// Interaction logic for ApplicationDetailsView.xaml
/// </summary>
public partial class ApplicationDetailsView : UserControl
{
    public ApplicationDetailsView(ApplicationDetailsViewModel applicationDetailsViewModel)
    {
        InitializeComponent();
        DataContext = applicationDetailsViewModel;
    }
}
