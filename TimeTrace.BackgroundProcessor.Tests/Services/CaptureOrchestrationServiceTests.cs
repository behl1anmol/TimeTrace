using Moq;
using Microsoft.Extensions.Logging;
using TimeTrace.BackgroundProcessor.Services;
using TimeTrace.Platform.Abstractions.Screenshot;
using timetrace.library.Models;
using timetrace.library.Repositories;
using TimeTrace.BackgroundProcessor.Configuration;

namespace TimeTrace.BackgroundProcessor.Tests.Services;

[TestFixture]
public class CaptureOrchestrationServiceTests
{
    private Mock<IScreenshotCaptureService> _captureServiceMock = null!;
    private Mock<IActiveWindowService> _activeWindowServiceMock = null!;
    private Mock<IProcessRepository> _processRepositoryMock = null!;
    private Mock<IImageRepository> _imageRepositoryMock = null!;
    private Mock<ICaptureConfigurationService> _configServiceMock = null!;
    private Mock<ILogger<CaptureOrchestrationService>> _loggerMock = null!;

    [SetUp]
    public void SetUp()
    {
        _captureServiceMock = new Mock<IScreenshotCaptureService>();
        _activeWindowServiceMock = new Mock<IActiveWindowService>();
        _processRepositoryMock = new Mock<IProcessRepository>();
        _imageRepositoryMock = new Mock<IImageRepository>();
        _configServiceMock = new Mock<ICaptureConfigurationService>();
        _loggerMock = new Mock<ILogger<CaptureOrchestrationService>>();
    }

    [Test]
    public void Constructor_WithValidDependencies_DoesNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrow(() => new CaptureOrchestrationService(
            _captureServiceMock.Object,
            _activeWindowServiceMock.Object,
            _processRepositoryMock.Object,
            _imageRepositoryMock.Object,
            _configServiceMock.Object,
            _loggerMock.Object));
    }

    [Test]
    public async Task CaptureAndPersistAsync_WhenServiceNotAvailable_ReturnsFailure()
    {
        // Arrange
        _captureServiceMock.Setup(x => x.IsAvailable).Returns(false);

        var service = new CaptureOrchestrationService(
            _captureServiceMock.Object,
            _activeWindowServiceMock.Object,
            _processRepositoryMock.Object,
            _imageRepositoryMock.Object,
            _configServiceMock.Object,
            _loggerMock.Object);

        // Act
        var result = await service.CaptureAndPersistAsync(CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("No screenshot capture tool available"));
    }

    [Test]
    public async Task CaptureAndPersistAsync_WhenCaptureSucceeds_ReturnsSuccess()
    {
        // Arrange
        var tempFolder = Path.GetTempPath();
        var tempFilePath = Path.Combine(tempFolder, "test.png");
        var settings = new CaptureSettings { TempFolderPath = tempFolder };
        
        var captureResult = new ScreenshotResult
        {
            Success = true,
            FilePath = tempFilePath,
            WindowInfo = new ActiveWindowInfo
            {
                WindowTitle = "Test Window",
                ProcessName = "testapp",
                ProcessId = 1234
            }
        };

        _captureServiceMock.Setup(x => x.IsAvailable).Returns(true);
        _configServiceMock.Setup(x => x.GetCurrentSettings()).Returns(settings);
        _captureServiceMock
            .Setup(x => x.CaptureActiveWindowAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(captureResult)
            .Callback<string, CancellationToken>((path, _) => File.WriteAllBytes(path, new byte[] { 0x89, 0x50 })); // Create dummy file
        
        _processRepositoryMock
            .Setup(x => x.GetProcessByName(It.IsAny<string>()))
            .Returns((timetrace.library.Models.Process?)null);
        _processRepositoryMock
            .Setup(x => x.AddProcess(It.IsAny<timetrace.library.Models.Process>()))
            .Returns(new timetrace.library.Models.Process { ProcessId = 1, Name = "testapp" });
        _processRepositoryMock
            .Setup(x => x.AddProcessDetail(It.IsAny<ProcessDetail>()))
            .Returns(new ProcessDetail { ProcessDetailId = 1 });
        _imageRepositoryMock
            .Setup(x => x.AddImage(It.IsAny<Image>()))
            .Returns(new Image { ImageId = 1, ImagePath = Path.Combine(tempFolder, "final.png") });

        var service = new CaptureOrchestrationService(
            _captureServiceMock.Object,
            _activeWindowServiceMock.Object,
            _processRepositoryMock.Object,
            _imageRepositoryMock.Object,
            _configServiceMock.Object,
            _loggerMock.Object);

        // Act
        var result = await service.CaptureAndPersistAsync(CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        _captureServiceMock.Verify(x => x.CaptureActiveWindowAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        
        // Cleanup
        if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
    }

    [Test]
    public async Task CaptureAndPersistAsync_WhenCaptureFails_ReturnsFailure()
    {
        // Arrange
        var tempFolder = Path.GetTempPath();
        var settings = new CaptureSettings { TempFolderPath = tempFolder };
        var captureResult = new ScreenshotResult
        {
            Success = false,
            ErrorMessage = "Screenshot tool not available"
        };

        _captureServiceMock.Setup(x => x.IsAvailable).Returns(true);
        _configServiceMock.Setup(x => x.GetCurrentSettings()).Returns(settings);
        _captureServiceMock
            .Setup(x => x.CaptureActiveWindowAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(captureResult);

        var service = new CaptureOrchestrationService(
            _captureServiceMock.Object,
            _activeWindowServiceMock.Object,
            _processRepositoryMock.Object,
            _imageRepositoryMock.Object,
            _configServiceMock.Object,
            _loggerMock.Object);

        // Act
        var result = await service.CaptureAndPersistAsync(CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        _imageRepositoryMock.Verify(x => x.AddImage(It.IsAny<Image>()), Times.Never);
    }
}
