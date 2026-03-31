namespace TimeTrace.Platform.Abstractions.Screenshot;

/// <summary>
/// Service for detecting the currently active/focused window.
/// </summary>
public interface IActiveWindowService
{
    /// <summary>
    /// Gets information about the currently active window.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Active window info, or null if no window is focused.</returns>
    Task<ActiveWindowInfo?> GetActiveWindowAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Indicates whether active window detection is supported on this system.
    /// </summary>
    bool IsSupported { get; }
}
