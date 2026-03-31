namespace timetrace.library.Utils;

public static class ConfigSettingConstants
{
    // Existing path configuration
    public const string FilePathConfigSettingIndex = "Paths";
    public const string ImagePathConfigSettingKey = "ImagePath";

    // Background processor capture configuration
    // TODO: Review and finalize these key names if needed
    public const string CaptureConfigSettingIndex = "ScreenshotCapture";
    public const string CaptureIntervalSecondsKey = "IntervalSeconds";
    public const string CaptureTempFolderPathKey = "TempFolderPath";
    public const string CaptureEnabledKey = "Enabled";
    public const string CaptureMaxTempFolderSizeMBKey = "MaxTempFolderSizeMB";
}