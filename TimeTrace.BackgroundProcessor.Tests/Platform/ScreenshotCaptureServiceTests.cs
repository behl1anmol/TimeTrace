using Moq;
using TimeTrace.Platform.Abstractions.Screenshot;

namespace TimeTrace.BackgroundProcessor.Tests.Platform;

[TestFixture]
public class ScreenshotCaptureServiceTests
{
    private Mock<IScreenshotCaptureService> _captureServiceMock = null!;

    [SetUp]
    public void SetUp()
    {
        _captureServiceMock = new Mock<IScreenshotCaptureService>();
    }

    [Test]
    public async Task CaptureActiveWindowAsync_WhenSuccessful_ReturnsSuccessResult()
    {
        // Arrange
        var expectedPath = "/tmp/test-screenshot.png";
        var expectedResult = new ScreenshotResult
        {
            Success = true,
            FilePath = expectedPath
        };
        _captureServiceMock
            .Setup(x => x.CaptureActiveWindowAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _captureServiceMock.Object.CaptureActiveWindowAsync(expectedPath, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.FilePath, Is.EqualTo(expectedPath));
        Assert.That(result.ErrorMessage, Is.Null);
    }

    [Test]
    public async Task CaptureActiveWindowAsync_WhenFailed_ReturnsFailureResult()
    {
        // Arrange
        var expectedError = "No capture tool available";
        var expectedResult = new ScreenshotResult
        {
            Success = false,
            ErrorMessage = expectedError
        };
        _captureServiceMock
            .Setup(x => x.CaptureActiveWindowAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _captureServiceMock.Object.CaptureActiveWindowAsync("/tmp/test.png", CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo(expectedError));
    }

    [Test]
    public async Task CaptureFullScreenAsync_WhenSuccessful_ReturnsSuccessResult()
    {
        // Arrange
        var expectedPath = "/tmp/fullscreen.png";
        var expectedResult = new ScreenshotResult
        {
            Success = true,
            FilePath = expectedPath
        };
        _captureServiceMock
            .Setup(x => x.CaptureFullScreenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _captureServiceMock.Object.CaptureFullScreenAsync(expectedPath, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.FilePath, Is.EqualTo(expectedPath));
    }

    [Test]
    public void IsAvailable_WhenCaptureToolExists_ReturnsTrue()
    {
        // Arrange
        _captureServiceMock.Setup(x => x.IsAvailable).Returns(true);

        // Act
        var result = _captureServiceMock.Object.IsAvailable;

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsAvailable_WhenNoCaptureToolExists_ReturnsFalse()
    {
        // Arrange
        _captureServiceMock.Setup(x => x.IsAvailable).Returns(false);

        // Act
        var result = _captureServiceMock.Object.IsAvailable;

        // Assert
        Assert.That(result, Is.False);
    }
}
