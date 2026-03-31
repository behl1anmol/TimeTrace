using Microsoft.Extensions.Logging;
using TimeTrace.Platform.Abstractions.Screenshot;
using SysProcess = System.Diagnostics.Process;
using SysProcessStartInfo = System.Diagnostics.ProcessStartInfo;

namespace TimeTrace.Platform.Linux.Screenshot;

/// <summary>
/// Linux implementation for detecting the currently active window.
/// Supports X11 via xdotool and Wayland via environment detection.
/// </summary>
public class LinuxActiveWindowService : IActiveWindowService
{
    private readonly ILogger<LinuxActiveWindowService> _logger;
    private readonly Lazy<bool> _isXdotoolAvailable;
    private readonly Lazy<bool> _isWayland;

    public LinuxActiveWindowService(ILogger<LinuxActiveWindowService> logger)
    {
        _logger = logger;
        _isXdotoolAvailable = new Lazy<bool>(CheckXdotoolAvailable);
        _isWayland = new Lazy<bool>(() =>
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY")));
    }

    public bool IsSupported => _isXdotoolAvailable.Value || _isWayland.Value;

    public async Task<ActiveWindowInfo?> GetActiveWindowAsync(CancellationToken cancellationToken = default)
    {
        // Try X11 first (most common on Linux desktops)
        if (_isXdotoolAvailable.Value && !_isWayland.Value)
        {
            return await GetActiveWindowX11Async(cancellationToken);
        }

        // Wayland - try to get at least basic info
        if (_isWayland.Value)
        {
            return await GetActiveWindowWaylandAsync(cancellationToken);
        }

        _logger.LogWarning("No active window detection method available");
        return null;
    }

    private async Task<ActiveWindowInfo?> GetActiveWindowX11Async(CancellationToken ct)
    {
        try
        {
            // Get active window ID
            var windowId = await RunCommandAsync("xdotool", "getactivewindow", ct);
            if (string.IsNullOrWhiteSpace(windowId))
            {
                _logger.LogDebug("No active window found (xdotool returned empty)");
                return null;
            }

            windowId = windowId.Trim();

            // Get window title
            var windowTitle = await RunCommandAsync("xdotool", $"getwindowname {windowId}", ct);

            // Get window PID
            var pidStr = await RunCommandAsync("xdotool", $"getwindowpid {windowId}", ct);
            if (!int.TryParse(pidStr?.Trim(), out var pid))
            {
                _logger.LogDebug("Could not get PID for window {WindowId}", windowId);
                pid = 0;
            }

            // Get process name from /proc
            var processName = GetProcessName(pid) ?? "unknown";
            var executablePath = GetExecutablePath(pid);

            return new ActiveWindowInfo
            {
                ProcessName = processName,
                WindowTitle = windowTitle?.Trim() ?? string.Empty,
                ProcessId = pid,
                ExecutablePath = executablePath,
                CapturedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active window via X11");
            return null;
        }
    }

    private async Task<ActiveWindowInfo?> GetActiveWindowWaylandAsync(CancellationToken ct)
    {
        // Wayland active window detection is compositor-specific
        // Try wlrctl if available (for wlroots-based compositors)
        try
        {
            var result = await RunCommandAsync("wlrctl", "toplevel focus", ct);
            if (!string.IsNullOrWhiteSpace(result))
            {
                // wlrctl output format varies, try to parse basic info
                _logger.LogDebug("wlrctl output: {Result}", result);
                
                // For now, return minimal info - Wayland doesn't expose as much
                return new ActiveWindowInfo
                {
                    ProcessName = "wayland-window",
                    WindowTitle = result.Trim(),
                    ProcessId = 0,
                    CapturedAt = DateTime.UtcNow
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "wlrctl not available or failed");
        }

        // Fallback: Try to get focused window from compositor via D-Bus
        // This is a simplified fallback - full implementation would use org.freedesktop.portal
        _logger.LogWarning("Wayland active window detection limited - consider using X11 or installing wlrctl");
        
        return new ActiveWindowInfo
        {
            ProcessName = "unknown",
            WindowTitle = "Wayland Window",
            ProcessId = 0,
            CapturedAt = DateTime.UtcNow
        };
    }

    private static string? GetProcessName(int pid)
    {
        if (pid <= 0) return null;

        try
        {
            var commPath = $"/proc/{pid}/comm";
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

    private static string? GetExecutablePath(int pid)
    {
        if (pid <= 0) return null;

        try
        {
            var exePath = $"/proc/{pid}/exe";
            if (File.Exists(exePath))
            {
                // /proc/PID/exe is a symlink to the actual executable
                var target = Path.GetFullPath(exePath);
                // ReadLink equivalent
                var linkInfo = new FileInfo(exePath);
                if (linkInfo.LinkTarget is not null)
                {
                    return linkInfo.LinkTarget;
                }
            }
        }
        catch
        {
            // Access denied or process exited
        }

        return null;
    }

    private static bool CheckXdotoolAvailable()
    {
        try
        {
            var psi = new SysProcessStartInfo("which", "xdotool")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = SysProcess.Start(psi);
            process?.WaitForExit(1000);
            return process?.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<string?> RunCommandAsync(string command, string arguments, CancellationToken ct)
    {
        try
        {
            var psi = new SysProcessStartInfo(command, arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = SysProcess.Start(psi);
            if (process is null) return null;

            var output = await process.StandardOutput.ReadToEndAsync(ct);
            await process.WaitForExitAsync(ct);

            return process.ExitCode == 0 ? output : null;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return null;
        }
    }
}
