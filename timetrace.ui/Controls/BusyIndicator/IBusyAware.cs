using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timetrace.ui.Controls.BusyIndicator;

public interface IBusyAware
{
    bool IsBusy
    {
        get;
    }
    string BusyMessage
    {
        get;
    }
}
