using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;

namespace TimeTrace.Platform.Abstractions.Extensions;

/// <summary>
/// Extension methods for registering platform-specific services.
/// </summary>
public static class PlatformServiceCollectionExtensions
{
    /// <summary>
    /// Registers platform-specific services based on the current runtime platform.
    /// Uses runtime detection to load the appropriate platform assembly.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="PlatformNotSupportedException">
    /// Thrown when no platform implementation is found for the current OS.
    /// </exception>
    public static IServiceCollection AddPlatformServices(this IServiceCollection services)
    {
        var registrations = LoadPlatformRegistrations();

        var currentPlatform = registrations.FirstOrDefault(r => r.IsCurrentPlatform);

        if (currentPlatform is null)
        {
            throw new PlatformNotSupportedException(
                $"No platform implementation found for {RuntimeInformation.OSDescription}. " +
                $"Ensure TimeTrace.Platform.Linux or TimeTrace.Platform.Windows is referenced.");
        }

        currentPlatform.RegisterServices(services);
        return services;
    }

    /// <summary>
    /// Explicitly registers a specific platform's services (useful for testing).
    /// </summary>
    /// <typeparam name="TRegistration">The platform registration type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddPlatformServices<TRegistration>(this IServiceCollection services)
        where TRegistration : IPlatformServiceRegistration, new()
    {
        new TRegistration().RegisterServices(services);
        return services;
    }

    private static IEnumerable<IPlatformServiceRegistration> LoadPlatformRegistrations()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch
                {
                    return Array.Empty<Type>();
                }
            })
            .Where(t => typeof(IPlatformServiceRegistration).IsAssignableFrom(t)
                        && !t.IsInterface && !t.IsAbstract)
            .Select(t => (IPlatformServiceRegistration)Activator.CreateInstance(t)!)
            .ToList();
    }
}
