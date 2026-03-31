using TimeTrace.Platform.Abstractions.Process;

namespace TimeTrace.Platform.Windows.Process;

/// <summary>
/// Windows implementation of process information service.
/// TODO: Implement using System.Diagnostics.Process and Win32 APIs
/// </summary>
public sealed class WindowsProcessInfoService : IProcessInfoService
{
    /// <inheritdoc />
    public ProcessInfo? GetProcessInfo(int processId)
    {
        // TODO: Implement using:
        // - Process.GetProcessById(processId)
        // - WMI for additional details
        throw new PlatformNotSupportedException(
            "Windows process info service is not yet implemented. " +
            "This stub exists for future development.");
    }

    /// <inheritdoc />
    public string? GetExecutablePath(int processId)
    {
        // TODO: Implement using:
        // - Process.GetProcessById(processId).MainModule?.FileName
        // - Handle access denied exceptions gracefully
        throw new PlatformNotSupportedException(
            "Windows process info service is not yet implemented. " +
            "This stub exists for future development.");
    }
}
