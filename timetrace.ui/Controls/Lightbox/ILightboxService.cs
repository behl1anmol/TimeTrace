using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timetrace.ui.Controls.Lightbox;

public interface ILightboxService
{
    Task<bool> ShowConfirmationAsync(string title, string message, string confirmText, string cancelText);
    Task ShowMessageAsync(string title, string message, string okText = "OK");
    Task<LightboxResult> ShowOptionsAsync(string title, string message, string primaryText, string secondaryText, string cancelText = "Cancel");
}
