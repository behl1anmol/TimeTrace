using Microsoft.Extensions.DependencyInjection;
using TimeTrace.Platform.Abstractions;
using TimeTrace.Platform.Abstractions.Process;
using TimeTrace.Platform.Abstractions.Screenshot;
using TimeTrace.Platform.Windows.Process;
using TimeTrace.Platform.Windows.Screenshot;

namespace TimeTrace.Platform.Windows.Extensions;

/// <summary>
/// Extension methods for registering Windows platform services.
/// </summary>
public static class WindowsPlatformServiceCollectionExtensions
{
    /// <summary>
    /// Adds Windows platform services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWindowsPlatformServices(this IServiceCollection services)
    {
        // Register Windows-specific implementations
        services.AddSingleton<IScreenshotCaptureService, WindowsScreenshotCaptureService>();
        services.AddSingleton<IActiveWindowService, WindowsActiveWindowService>();
        services.AddSingleton<IProcessInfoService, WindowsProcessInfoService>();

        return services;
    }
}

/// <summary>
/// Windows platform service registration marker for auto-detection.
/// </summary>
public sealed class WindowsPlatformServiceRegistration : IPlatformServiceRegistration
{
    /// <inheritdoc />
    public bool IsCurrentPlatform => OperatingSystem.IsWindows();

    /// <inheritdoc />
    public void RegisterServices(IServiceCollection services)
    {
        services.AddWindowsPlatformServices();
    }
}
