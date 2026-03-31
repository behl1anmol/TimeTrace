namespace TimeTrace.BackgroundProcessor.Configuration;

/// <summary>
/// Settings for screenshot capture behavior.
/// </summary>
public sealed class CaptureSettings
{
    /// <summary>Interval between captures in seconds. Default: 30</summary>
    public int IntervalSeconds { get; set; } = 30;

    /// <summary>Path to temp folder for screenshots before persistence. Default: ~/.local/share/timetrace/captures</summary>
    public string TempFolderPath { get; set; } = GetDefaultTempPath();

    /// <summary>Whether capture is enabled. Default: true</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Maximum temp folder size in MB before cleanup. Default: 500</summary>
    public int MaxTempFolderSizeMB { get; set; } = 500;

    /// <summary>
    /// Gets the capture interval as a TimeSpan.
    /// </summary>
    public TimeSpan CaptureInterval => TimeSpan.FromSeconds(Math.Max(1, IntervalSeconds));

    private static string GetDefaultTempPath()
    {
        // XDG Base Directory spec: ~/.local/share/timetrace/captures
        var xdgDataHome = Environment.GetEnvironmentVariable("XDG_DATA_HOME")
            ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
        return Path.Combine(xdgDataHome, "timetrace", "captures");
    }

    /// <summary>
    /// Expands ~ to home directory in paths.
    /// </summary>
    public static string ExpandPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return path;
        return path.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
    }
}
