namespace TimeTrace.Platform.Abstractions.Screenshot;

/// <summary>
/// Result of a screenshot capture operation.
/// </summary>
public sealed record ScreenshotResult
{
    public required bool Success { get; init; }

    /// <summary>Path to saved screenshot file (only if Success is true).</summary>
    public string? FilePath { get; init; }

    /// <summary>Error message (only if Success is false).</summary>
    public string? ErrorMessage { get; init; }

    /// <summary>Active window info at time of capture.</summary>
    public ActiveWindowInfo? WindowInfo { get; init; }

    /// <summary>Duration of the capture operation.</summary>
    public TimeSpan Duration { get; init; }

    /// <summary>Creates a successful result.</summary>
    public static ScreenshotResult Succeeded(string filePath, ActiveWindowInfo? windowInfo, TimeSpan duration) =>
        new() { Success = true, FilePath = filePath, WindowInfo = windowInfo, Duration = duration };

    /// <summary>Creates a failed result.</summary>
    public static ScreenshotResult Failed(string error) =>
        new() { Success = false, ErrorMessage = error };
}
