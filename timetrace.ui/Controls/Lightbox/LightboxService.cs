using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timetrace.ui.Controls.Lightbox;

public class LightboxService : ILightboxService
{
    private readonly ILightboxHost _lightboxHost;

    public LightboxService(ILightboxHost lightboxHost)
    {
        _lightboxHost = lightboxHost;
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message, string confirmText, string cancelText)
    {
        var viewModel = new LightboxViewModel
        {
            Title = title,
            Message = message,
            PrimaryButtonText = confirmText,
            CancelButtonText = cancelText,
            ShowCancelButton = true,
            ShowSecondaryButton = false
        };

        var result = await _lightboxHost.ShowLightboxAsync<LightboxResult>(viewModel);
        return result == LightboxResult.Primary;
    }

    public async Task ShowMessageAsync(string title, string message, string okText = "OK")
    {
        var viewModel = new LightboxViewModel
        {
            Title = title,
            Message = message,
            PrimaryButtonText = okText,
            ShowSecondaryButton = false,
            ShowCancelButton = false
        };

        await _lightboxHost.ShowLightboxAsync<LightboxResult>(viewModel);
    }

    public async Task<LightboxResult> ShowOptionsAsync(string title, string message, string primaryText, string secondaryText, string cancelText = "Cancel")
    {
        var viewModel = new LightboxViewModel
        {
            Title = title,
            Message = message,
            PrimaryButtonText = primaryText,
            SecondaryButtonText = secondaryText,
            CancelButtonText = cancelText,
            ShowSecondaryButton = true,
            ShowCancelButton = !string.IsNullOrEmpty(cancelText)
        };

        return await _lightboxHost.ShowLightboxAsync<LightboxResult>(viewModel);
    }
}
