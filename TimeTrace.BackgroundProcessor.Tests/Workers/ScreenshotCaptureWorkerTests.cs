using Moq;
using Microsoft.Extensions.Logging;
using TimeTrace.BackgroundProcessor.Workers;
using TimeTrace.BackgroundProcessor.Services;
using TimeTrace.BackgroundProcessor.Configuration;

namespace TimeTrace.BackgroundProcessor.Tests.Workers;

[TestFixture]
public class ScreenshotCaptureWorkerTests
{
    private Mock<ICaptureOrchestrationService> _orchestrationServiceMock = null!;
    private Mock<ICaptureConfigurationService> _configServiceMock = null!;
    private Mock<ILogger<ScreenshotCaptureWorker>> _loggerMock = null!;

    [SetUp]
    public void SetUp()
    {
        _orchestrationServiceMock = new Mock<ICaptureOrchestrationService>();
        _configServiceMock = new Mock<ICaptureConfigurationService>();
        _loggerMock = new Mock<ILogger<ScreenshotCaptureWorker>>();
    }

    [Test]
    public void Constructor_WithValidDependencies_DoesNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrow(() => new ScreenshotCaptureWorker(
            _orchestrationServiceMock.Object,
            _configServiceMock.Object,
            _loggerMock.Object));
    }

    [Test]
    public async Task StartAsync_EnsuresDefaultConfiguration()
    {
        // Arrange
        var settings = new CaptureSettings
        {
            Enabled = false,
            IntervalSeconds = 30,
            TempFolderPath = "/tmp/timetrace"
        };
        _configServiceMock.Setup(x => x.GetCurrentSettings()).Returns(settings);

        var worker = new ScreenshotCaptureWorker(
            _orchestrationServiceMock.Object,
            _configServiceMock.Object,
            _loggerMock.Object);

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        // Act
        await worker.StartAsync(cts.Token);
        await Task.Delay(50);
        await worker.StopAsync(CancellationToken.None);

        // Assert
        _configServiceMock.Verify(x => x.EnsureDefaultConfiguration(), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_WhenDisabled_DoesNotCapture()
    {
        // Arrange
        var settings = new CaptureSettings
        {
            Enabled = false,
            IntervalSeconds = 30,
            TempFolderPath = "/tmp/timetrace"
        };
        _configServiceMock.Setup(x => x.GetCurrentSettings()).Returns(settings);

        var worker = new ScreenshotCaptureWorker(
            _orchestrationServiceMock.Object,
            _configServiceMock.Object,
            _loggerMock.Object);

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));

        // Act
        await worker.StartAsync(cts.Token);
        await Task.Delay(100);
        await worker.StopAsync(CancellationToken.None);

        // Assert
        _orchestrationServiceMock.Verify(
            x => x.CaptureAndPersistAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task ExecuteAsync_WhenCancelled_StopsGracefully()
    {
        // Arrange
        var settings = new CaptureSettings
        {
            Enabled = true,
            IntervalSeconds = 60,
            TempFolderPath = "/tmp/timetrace"
        };
        _configServiceMock.Setup(x => x.GetCurrentSettings()).Returns(settings);

        var worker = new ScreenshotCaptureWorker(
            _orchestrationServiceMock.Object,
            _configServiceMock.Object,
            _loggerMock.Object);

        using var cts = new CancellationTokenSource();

        // Act
        var startTask = worker.StartAsync(cts.Token);
        await Task.Delay(50);
        cts.Cancel();

        // Assert - should not throw
        Assert.DoesNotThrowAsync(async () => await worker.StopAsync(CancellationToken.None));
    }
}
