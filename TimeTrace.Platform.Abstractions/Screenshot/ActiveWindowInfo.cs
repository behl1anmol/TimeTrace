namespace TimeTrace.Platform.Abstractions.Screenshot;

/// <summary>
/// Information about the currently active window.
/// </summary>
public sealed record ActiveWindowInfo
{
    /// <summary>Process name (e.g., "firefox", "code").</summary>
    public required string ProcessName { get; init; }

    /// <summary>Window title (e.g., "GitHub - Mozilla Firefox").</summary>
    public required string WindowTitle { get; init; }

    /// <summary>OS process identifier.</summary>
    public required int ProcessId { get; init; }

    /// <summary>Executable path if available.</summary>
    public string? ExecutablePath { get; init; }

    /// <summary>Timestamp when window info was captured.</summary>
    public DateTime CapturedAt { get; init; } = DateTime.UtcNow;
}
