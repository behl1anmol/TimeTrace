using Microsoft.Extensions.DependencyInjection;

namespace TimeTrace.Platform.Abstractions;

/// <summary>
/// Marker interface for platform-specific service registration.
/// Each platform project implements this to register its services.
/// </summary>
public interface IPlatformServiceRegistration
{
    /// <summary>
    /// Registers platform-specific services with the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    void RegisterServices(IServiceCollection services);

    /// <summary>
    /// Returns true if this platform registration applies to the current OS.
    /// </summary>
    bool IsCurrentPlatform { get; }
}
