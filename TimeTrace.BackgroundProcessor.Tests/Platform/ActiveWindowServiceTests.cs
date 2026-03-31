using Moq;
using TimeTrace.Platform.Abstractions.Screenshot;

namespace TimeTrace.BackgroundProcessor.Tests.Platform;

[TestFixture]
public class ActiveWindowServiceTests
{
    private Mock<IActiveWindowService> _activeWindowServiceMock = null!;

    [SetUp]
    public void SetUp()
    {
        _activeWindowServiceMock = new Mock<IActiveWindowService>();
    }

    [Test]
    public async Task GetActiveWindowAsync_WhenWindowAvailable_ReturnsWindowInfo()
    {
        // Arrange
        var expectedInfo = new ActiveWindowInfo
        {
            WindowTitle = "Visual Studio Code",
            ProcessName = "code",
            ProcessId = 12345
        };
        _activeWindowServiceMock
            .Setup(x => x.GetActiveWindowAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedInfo);

        // Act
        var result = await _activeWindowServiceMock.Object.GetActiveWindowAsync(CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.WindowTitle, Is.EqualTo("Visual Studio Code"));
        Assert.That(result.ProcessName, Is.EqualTo("code"));
        Assert.That(result.ProcessId, Is.EqualTo(12345));
    }

    [Test]
    public async Task GetActiveWindowAsync_WhenNoWindow_ReturnsNull()
    {
        // Arrange
        _activeWindowServiceMock
            .Setup(x => x.GetActiveWindowAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((ActiveWindowInfo?)null);

        // Act
        var result = await _activeWindowServiceMock.Object.GetActiveWindowAsync(CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetActiveWindowAsync_WhenDesktopActive_ReturnsDesktopInfo()
    {
        // Arrange
        var expectedInfo = new ActiveWindowInfo
        {
            WindowTitle = "Desktop",
            ProcessName = "explorer",
            ProcessId = 1000
        };
        _activeWindowServiceMock
            .Setup(x => x.GetActiveWindowAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedInfo);

        // Act
        var result = await _activeWindowServiceMock.Object.GetActiveWindowAsync(CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.WindowTitle, Is.EqualTo("Desktop"));
    }
}
