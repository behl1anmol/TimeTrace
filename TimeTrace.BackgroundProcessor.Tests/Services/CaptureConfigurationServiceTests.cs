using Moq;
using Microsoft.Extensions.Logging;
using TimeTrace.BackgroundProcessor.Services;
using TimeTrace.BackgroundProcessor.Configuration;
using timetrace.library.Repositories;
using timetrace.library.Utils;

namespace TimeTrace.BackgroundProcessor.Tests.Services;

[TestFixture]
public class CaptureConfigurationServiceTests
{
    private Mock<IConfigurationRepository> _configurationRepositoryMock = null!;
    private Mock<ILogger<CaptureConfigurationService>> _loggerMock = null!;

    [SetUp]
    public void SetUp()
    {
        _configurationRepositoryMock = new Mock<IConfigurationRepository>();
        _loggerMock = new Mock<ILogger<CaptureConfigurationService>>();
    }

    [Test]
    public void Constructor_WithValidDependencies_DoesNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrow(() => new CaptureConfigurationService(
            _configurationRepositoryMock.Object,
            _loggerMock.Object));
    }

    [Test]
    public void GetCurrentSettings_WithValidConfig_ReturnsSettings()
    {
        // Arrange
        var configValues = new Dictionary<string, string>
        {
            { ConfigSettingConstants.CaptureIntervalSecondsKey, "30" },
            { ConfigSettingConstants.CaptureTempFolderPathKey, "/tmp/timetrace" },
            { ConfigSettingConstants.CaptureEnabledKey, "true" },
            { ConfigSettingConstants.CaptureMaxTempFolderSizeMBKey, "500" }
        };

        _configurationRepositoryMock
            .Setup(x => x.FetchConfigurationSettingKeyValueByIndex(ConfigSettingConstants.CaptureConfigSettingIndex))
            .Returns(configValues);

        var service = new CaptureConfigurationService(
            _configurationRepositoryMock.Object,
            _loggerMock.Object);

        // Act
        var result = service.GetCurrentSettings();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IntervalSeconds, Is.EqualTo(30));
        Assert.That(result.TempFolderPath, Is.EqualTo("/tmp/timetrace"));
        Assert.That(result.Enabled, Is.True);
        Assert.That(result.MaxTempFolderSizeMB, Is.EqualTo(500));
    }

    [Test]
    public void GetCurrentSettings_WithMissingConfig_ReturnsDefaults()
    {
        // Arrange
        _configurationRepositoryMock
            .Setup(x => x.FetchConfigurationSettingKeyValueByIndex(It.IsAny<string>()))
            .Returns(new Dictionary<string, string>());

        var service = new CaptureConfigurationService(
            _configurationRepositoryMock.Object,
            _loggerMock.Object);

        // Act
        var result = service.GetCurrentSettings();

        // Assert
        Assert.That(result, Is.Not.Null);
        // Should return default values
        Assert.That(result.IntervalSeconds, Is.GreaterThan(0));
        Assert.That(result.TempFolderPath, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void GetCurrentSettings_WithInvalidInterval_UsesDefault()
    {
        // Arrange
        var configValues = new Dictionary<string, string>
        {
            { ConfigSettingConstants.CaptureIntervalSecondsKey, "invalid" }
        };

        _configurationRepositoryMock
            .Setup(x => x.FetchConfigurationSettingKeyValueByIndex(ConfigSettingConstants.CaptureConfigSettingIndex))
            .Returns(configValues);

        var service = new CaptureConfigurationService(
            _configurationRepositoryMock.Object,
            _loggerMock.Object);

        // Act
        var result = service.GetCurrentSettings();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IntervalSeconds, Is.GreaterThan(0)); // Should use default
    }

    [Test]
    public void EnsureDefaultConfiguration_AddsDefaultsWhenMissing()
    {
        // Arrange
        _configurationRepositoryMock
            .Setup(x => x.FetchConfigurationSettingKeyValueByIndex(ConfigSettingConstants.CaptureConfigSettingIndex))
            .Returns(new Dictionary<string, string>());

        var service = new CaptureConfigurationService(
            _configurationRepositoryMock.Object,
            _loggerMock.Object);

        // Act
        service.EnsureDefaultConfiguration();

        // Assert
        _configurationRepositoryMock.Verify(
            x => x.AddConfigurationSetting(ConfigSettingConstants.CaptureConfigSettingIndex),
            Times.Once);
        _configurationRepositoryMock.Verify(
            x => x.AddConfigurationSettingDetail(
                ConfigSettingConstants.CaptureConfigSettingIndex,
                ConfigSettingConstants.CaptureIntervalSecondsKey,
                It.IsAny<string>()),
            Times.Once);
    }
}
