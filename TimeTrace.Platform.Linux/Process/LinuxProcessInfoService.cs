using TimeTrace.Platform.Abstractions.Process;

namespace TimeTrace.Platform.Linux.Process;

/// <summary>
/// Linux implementation for gathering process information.
/// Uses /proc filesystem for process details.
/// </summary>
public class LinuxProcessInfoService : IProcessInfoService
{
    public ProcessInfo? GetProcessInfo(int processId)
    {
        if (processId <= 0) return null;

        var procPath = $"/proc/{processId}";
        if (!Directory.Exists(procPath)) return null;

        try
        {
            var processName = GetProcessName(processId);
            if (processName is null) return null;

            return new ProcessInfo
            {
                ProcessId = processId,
                ProcessName = processName,
                ExecutablePath = GetExecutablePath(processId),
                CommandLine = GetCommandLine(processId),
                StartTime = GetStartTime(processId)
            };
        }
        catch
        {
            return null;
        }
    }

    public string? GetExecutablePath(int processId)
    {
        if (processId <= 0) return null;

        try
        {
            var exeLink = $"/proc/{processId}/exe";
            if (!File.Exists(exeLink)) return null;

            var linkInfo = new FileInfo(exeLink);
            return linkInfo.LinkTarget;
        }
        catch
        {
            return null;
        }
    }

    private static string? GetProcessName(int processId)
    {
        try
        {
            var commPath = $"/proc/{processId}/comm";
            if (File.Exists(commPath))
            {
                return File.ReadAllText(commPath).Trim();
            }
        }
        catch
        {
            // Process might have exited
        }

        return null;
    }

    private static string? GetCommandLine(int processId)
    {
        try
        {
            var cmdlinePath = $"/proc/{processId}/cmdline";
            if (File.Exists(cmdlinePath))
            {
                var cmdline = File.ReadAllText(cmdlinePath);
                // Arguments are null-separated
                return cmdline.Replace('\0', ' ').Trim();
            }
        }
        catch
        {
            // Process might have exited
        }

        return null;
    }

    private static DateTime? GetStartTime(int processId)
    {
        try
        {
            var statPath = $"/proc/{processId}/stat";
            if (File.Exists(statPath))
            {
                var stat = File.ReadAllText(statPath);
                // Format: pid (comm) state ... starttime (field 22)
                // This is complex to parse correctly, return null for now
                // A full implementation would need to handle the boot time
            }
        }
        catch
        {
            // Process might have exited
        }

        return null;
    }
}
