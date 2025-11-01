using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using TaskForge.API.Controllers;
using TaskForge.Application.Core;
using Tests.TaskForge.API.Stub;
using Xunit;

namespace Tests.TaskForge.API;

/// <summary>
/// Unit tests for VersionController
/// </summary>
public class VersionControllerTests
{
    #region Helper Methods

    private VersionController CreateController(IWebHostEnvironment environment = null, ILogger<VersionController> logger = null)
    {
        var mockEnvironment = environment ?? new Mock<IWebHostEnvironment>().Object;
        var mockLogger = logger ?? new Mock<ILogger<VersionController>>().Object;
        return new VersionController(mockEnvironment, mockLogger);
    }

    private StubVersionController CreateStubController(IWebHostEnvironment environment = null, ILogger<VersionController> logger = null, string versionString = "1.0.0.0")
    {
        var mockEnvironment = environment ?? new Mock<IWebHostEnvironment>().Object;
        var mockLogger = logger ?? new Mock<ILogger<VersionController>>().Object;
        return new StubVersionController(mockEnvironment, mockLogger)
        {
            VersionString = versionString
        };
    }

    #endregion

    #region GetVersion Tests

    [Fact]
    public void GetVersion_WhenVersionAvailable_ReturnsOkWithVersionInfo()
    {
        // Arrange
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        mockEnvironment.Setup(e => e.EnvironmentName).Returns("Development");
        var mockLogger = new Mock<ILogger<VersionController>>();
        
        var controller = CreateStubController(mockEnvironment.Object, mockLogger.Object, "1.2.3.4");

        // Act
        var result = controller.GetVersion();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var versionResult = Assert.IsType<VersionResult>(okResult.Value);
        Assert.Equal("Development", versionResult.Environment);
        Assert.Equal("1.2.3.4", versionResult.Version);
    }

    [Fact]
    public void GetVersion_WhenVersionRetrieved_LogsInformation()
    {
        // Arrange
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
        var mockLogger = new Mock<ILogger<VersionController>>();
        
        var controller = CreateStubController(mockEnvironment.Object, mockLogger.Object, "2.0.0.0");

        // Act
        controller.GetVersion();

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Retrieved application version")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void GetVersion_WhenVersionIsNull_LogsErrorAndReturnsFailure()
    {
        // Arrange
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        mockEnvironment.Setup(e => e.EnvironmentName).Returns("Development");
        var mockLogger = new Mock<ILogger<VersionController>>();
        
        // Create stub that throws exception when version is null to simulate failure
        var controller = CreateStubController(mockEnvironment.Object, mockLogger.Object);
        controller.VersionString = null; // This will cause exception in GetVersionFromAssembly

        // Act
        var result = controller.GetVersion();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
        
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to retrieve application version")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void GetVersion_WhenEnvironmentIsSet_CorrectlyReturnsEnvironmentName()
    {
        // Arrange
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        mockEnvironment.Setup(e => e.EnvironmentName).Returns("Staging");
        var mockLogger = new Mock<ILogger<VersionController>>();
        
        var controller = CreateStubController(mockEnvironment.Object, mockLogger.Object);

        // Act
        var result = controller.GetVersion();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var versionResult = Assert.IsType<VersionResult>(okResult.Value);
        Assert.Equal("Staging", versionResult.Environment);
    }

    #endregion
}
