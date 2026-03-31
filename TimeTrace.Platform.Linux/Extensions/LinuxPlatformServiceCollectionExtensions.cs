using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using TimeTrace.Platform.Abstractions;
using TimeTrace.Platform.Abstractions.Process;
using TimeTrace.Platform.Abstractions.Screenshot;
using TimeTrace.Platform.Linux.Process;
using TimeTrace.Platform.Linux.Screenshot;
using TimeTrace.Platform.Linux.Screenshot.Capture;

namespace TimeTrace.Platform.Linux.Extensions;

/// <summary>
/// Platform service registration for Linux.
/// </summary>
public sealed class LinuxPlatformServiceRegistration : IPlatformServiceRegistration
{
    public bool IsCurrentPlatform => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    public void RegisterServices(IServiceCollection services)
    {
        // Active window detection
        services.AddSingleton<IActiveWindowService, LinuxActiveWindowService>();

        // Screenshot capture with chain-of-responsibility
        services.AddSingleton<IScreenshotCaptureService, LinuxScreenshotCaptureService>();

        // Individual capture strategies (order matters for DI enumeration)
        services.AddSingleton<ICaptureStrategy, GnomeScreenshotCapture>();
        services.AddSingleton<ICaptureStrategy, GrimCapture>();
        services.AddSingleton<ICaptureStrategy, ScrotCapture>();

        // Process info
        services.AddSingleton<IProcessInfoService, LinuxProcessInfoService>();
    }
}

/// <summary>
/// Convenience extension for explicit Linux registration.
/// </summary>
public static class LinuxServiceCollectionExtensions
{
    /// <summary>
    /// Explicitly adds Linux platform services (useful for testing or explicit configuration).
    /// </summary>
    public static IServiceCollection AddLinuxPlatformServices(this IServiceCollection services)
    {
        new LinuxPlatformServiceRegistration().RegisterServices(services);
        return services;
    }
}
