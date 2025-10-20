using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace timetrace.ui.Controls.BusyIndicator;

public abstract partial class BusyAwareViewModel : ObservableObject, IBusyAware
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _busyMessage = string.Empty;
}