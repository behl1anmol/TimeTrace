using Microsoft.Extensions.Logging;
using timetrace.library.Repositories;
using timetrace.library.Utils;
using TimeTrace.BackgroundProcessor.Configuration;

namespace TimeTrace.BackgroundProcessor.Services;

/// <summary>
/// Service for reading capture configuration from the database.
/// </summary>
public interface ICaptureConfigurationService
{
    /// <summary>
    /// Gets the current capture settings from the database.
    /// </summary>
    CaptureSettings GetCurrentSettings();

    /// <summary>
    /// Ensures default configuration exists in the database.
    /// </summary>
    void EnsureDefaultConfiguration();
}

/// <summary>
/// Implementation of ICaptureConfigurationService using the existing ConfigurationRepository.
/// </summary>
public class CaptureConfigurationService : ICaptureConfigurationService
{
    private readonly IConfigurationRepository _configRepo;
    private readonly ILogger<CaptureConfigurationService> _logger;

    public CaptureConfigurationService(
        IConfigurationRepository configRepo,
        ILogger<CaptureConfigurationService> logger)
    {
        _configRepo = configRepo;
        _logger = logger;
    }

    public CaptureSettings GetCurrentSettings()
    {
        var settings = new CaptureSettings();

        try
        {
            var configValues = _configRepo.FetchConfigurationSettingKeyValueByIndex(
                ConfigSettingConstants.CaptureConfigSettingIndex);

            if (configValues.Count == 0)
            {
                _logger.LogDebug("No capture configuration found, using defaults");
                return settings;
            }

            // Parse interval
            if (configValues.TryGetValue(ConfigSettingConstants.CaptureIntervalSecondsKey, out var intervalStr)
                && int.TryParse(intervalStr, out var intervalValue)
                && intervalValue > 0)
            {
                settings.IntervalSeconds = intervalValue;
            }

            // Parse temp folder path
            if (configValues.TryGetValue(ConfigSettingConstants.CaptureTempFolderPathKey, out var tempPath)
                && !string.IsNullOrWhiteSpace(tempPath))
            {
                settings.TempFolderPath = CaptureSettings.ExpandPath(tempPath);
            }

            // Parse enabled flag
            if (configValues.TryGetValue(ConfigSettingConstants.CaptureEnabledKey, out var enabledStr)
                && bool.TryParse(enabledStr, out var enabledValue))
            {
                settings.Enabled = enabledValue;
            }

            // Parse max temp folder size
            if (configValues.TryGetValue(ConfigSettingConstants.CaptureMaxTempFolderSizeMBKey, out var maxSizeStr)
                && int.TryParse(maxSizeStr, out var maxSizeValue)
                && maxSizeValue > 0)
            {
                settings.MaxTempFolderSizeMB = maxSizeValue;
            }

            _logger.LogDebug("Loaded capture settings: Interval={Interval}s, Enabled={Enabled}, TempPath={TempPath}",
                settings.IntervalSeconds, settings.Enabled, settings.TempFolderPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading capture configuration, using defaults");
        }

        return settings;
    }

    public void EnsureDefaultConfiguration()
    {
        try
        {
            // Add the configuration setting index if it doesn't exist
            _configRepo.AddConfigurationSetting(ConfigSettingConstants.CaptureConfigSettingIndex);

            // Check if settings exist, if not add defaults
            var existingSettings = _configRepo.FetchConfigurationSettingKeyValueByIndex(
                ConfigSettingConstants.CaptureConfigSettingIndex);

            var defaults = new CaptureSettings();

            if (!existingSettings.ContainsKey(ConfigSettingConstants.CaptureIntervalSecondsKey))
            {
                _configRepo.AddConfigurationSettingDetail(
                    ConfigSettingConstants.CaptureConfigSettingIndex,
                    ConfigSettingConstants.CaptureIntervalSecondsKey,
                    defaults.IntervalSeconds.ToString());
            }

            if (!existingSettings.ContainsKey(ConfigSettingConstants.CaptureTempFolderPathKey))
            {
                _configRepo.AddConfigurationSettingDetail(
                    ConfigSettingConstants.CaptureConfigSettingIndex,
                    ConfigSettingConstants.CaptureTempFolderPathKey,
                    defaults.TempFolderPath);
            }

            if (!existingSettings.ContainsKey(ConfigSettingConstants.CaptureEnabledKey))
            {
                _configRepo.AddConfigurationSettingDetail(
                    ConfigSettingConstants.CaptureConfigSettingIndex,
                    ConfigSettingConstants.CaptureEnabledKey,
                    defaults.Enabled.ToString().ToLowerInvariant());
            }

            if (!existingSettings.ContainsKey(ConfigSettingConstants.CaptureMaxTempFolderSizeMBKey))
            {
                _configRepo.AddConfigurationSettingDetail(
                    ConfigSettingConstants.CaptureConfigSettingIndex,
                    ConfigSettingConstants.CaptureMaxTempFolderSizeMBKey,
                    defaults.MaxTempFolderSizeMB.ToString());
            }

            _logger.LogInformation("Ensured default capture configuration exists");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ensuring default configuration");
        }
    }
}
