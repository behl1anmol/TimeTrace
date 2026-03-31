namespace TimeTrace.Platform.Abstractions.Process;

/// <summary>
/// Platform-agnostic service for gathering process information.
/// </summary>
public interface IProcessInfoService
{
    /// <summary>
    /// Gets detailed information about a process by its ID.
    /// </summary>
    /// <param name="processId">The process ID.</param>
    /// <returns>Process info, or null if not found.</returns>
    ProcessInfo? GetProcessInfo(int processId);

    /// <summary>
    /// Gets the executable path for a process by its ID.
    /// </summary>
    /// <param name="processId">The process ID.</param>
    /// <returns>Executable path, or null if not accessible.</returns>
    string? GetExecutablePath(int processId);
}

/// <summary>
/// Information about a running process.
/// </summary>
public sealed record ProcessInfo
{
    public required int ProcessId { get; init; }
    public required string ProcessName { get; init; }
    public string? ExecutablePath { get; init; }
    public string? CommandLine { get; init; }
    public DateTime? StartTime { get; init; }
}
