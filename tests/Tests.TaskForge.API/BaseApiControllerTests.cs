/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TaskForge.API.Controllers;
using TaskForge.Application.Core;
using Xunit;

namespace Tests.TaskForge.API;

/// <summary>
/// Unit tests for BaseApiController
/// </summary>
public class BaseApiControllerTests
{
    #region Test Controller Implementation

    /// <summary>
    /// Test controller implementation for testing BaseApiController
    /// </summary>
    private class TestController : BaseApiController
    {
        public IActionResult TestHandleResult<T>(Result<T> result)
        {
            return HandleResult(result);
        }

        public IMediator TestMediator => Mediator;
    }

    #endregion

    #region HandleResult Tests - Success Cases

    [Fact]
    public void HandleResult_WhenSuccessWithValue_ReturnsOk()
    {
        // Arrange
        var controller = CreateController();
        var testValue = "test value";
        var result = Result<string>.Success(testValue);

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(testValue, okResult.Value);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public void HandleResult_WhenSuccessWithObject_ReturnsOk()
    {
        // Arrange
        var controller = CreateController();
        var testObject = new { Id = Guid.NewGuid(), Name = "Test" };
        var result = Result<object>.Success(testObject);

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(testObject, okResult.Value);
    }

    [Fact]
    public void HandleResult_WhenSuccessWithNullValue_ReturnsNotFound()
    {
        // Arrange
        var controller = CreateController();
        var result = Result<string>.Success(null);

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(actionResult);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public void HandleResult_WhenSuccessWithDefaultValueType_ReturnsNotFound()
    {
        // Arrange
        var controller = CreateController();
        var result = Result<int>.Success(default(int)); // 0

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(actionResult);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public void HandleResult_WhenSuccessWithZeroValue_ReturnsNotFound()
    {
        // Arrange
        var controller = CreateController();
        var result = Result<int>.Success(0);

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(actionResult);
    }

    #endregion

    #region HandleResult Tests - Failure Cases

    [Fact]
    public void HandleResult_WhenFailure_ReturnsBadRequest()
    {
        // Arrange
        var controller = CreateController();
        var errorMessage = "Something went wrong";
        var result = Result<string>.Failure(errorMessage);

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(errorMessage, badRequestResult.Value);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void HandleResult_WhenFailureWithEmptyErrorMessage_ReturnsBadRequest()
    {
        // Arrange
        var controller = CreateController();
        var result = Result<string>.Failure("");

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("", badRequestResult.Value);
    }

    [Fact]
    public void HandleResult_WhenFailureWithNullErrorMessage_ReturnsBadRequest()
    {
        // Arrange
        var controller = CreateController();
        var result = new Result<string>
        {
            IsSuccess = false,
            Error = null,
            Value = null
        };

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Null(badRequestResult.Value);
    }

    #endregion

    #region HandleResult Tests - Edge Cases

    [Fact]
    public void HandleResult_WhenResultIsNull_ReturnsInternalServerError()
    {
        // Arrange
        var controller = CreateController();
        Result<string> result = null;

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("Internal server error: result is null", statusCodeResult.Value);
    }

    [Fact]
    public void HandleResult_WhenSuccessWithGuid_ReturnsOk()
    {
        // Arrange
        var controller = CreateController();
        var testGuid = Guid.NewGuid();
        var result = Result<Guid>.Success(testGuid);

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(testGuid, okResult.Value);
    }

    [Fact]
    public void HandleResult_WhenSuccessWithEmptyGuid_ReturnsOk()
    {
        // Arrange
        var controller = CreateController();
        var result = Result<Guid>.Success(Guid.Empty);

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        // Guid.Empty != null, so Ok should be returned (not NotFound)
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(Guid.Empty, okResult.Value);
    }

    [Fact]
    public void HandleResult_WhenSuccessWithEmptyString_ReturnsOk()
    {
        // Arrange
        var controller = CreateController();
        var result = Result<string>.Success("");

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        // Empty string != null, so Ok should be returned
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal("", okResult.Value);
    }

    [Fact]
    public void HandleResult_WhenSuccessWithList_ReturnsOk()
    {
        // Arrange
        var controller = CreateController();
        var testList = new List<string> { "item1", "item2" };
        var result = Result<List<string>>.Success(testList);

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(testList, okResult.Value);
    }

    [Fact]
    public void HandleResult_WhenSuccessWithEmptyList_ReturnsOk()
    {
        // Arrange
        var controller = CreateController();
        var emptyList = new List<string>();
        var result = Result<List<string>>.Success(emptyList);

        // Act
        var actionResult = controller.TestHandleResult(result);

        // Assert
        // Empty list != null, so Ok should be returned
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedList = Assert.IsType<List<string>>(okResult.Value);
        Assert.Empty(returnedList);
    }

    #endregion

    #region Mediator Property Tests

    [Fact]
    public void Mediator_WhenHttpContextHasService_ReturnsMediator()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var serviceProvider = new ServiceCollection()
            .AddSingleton(mockMediator.Object)
            .BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        var controller = new TestController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var mediator = controller.TestMediator;

        // Assert
        Assert.NotNull(mediator);
        Assert.Equal(mockMediator.Object, mediator);
    }

    [Fact]
    public void Mediator_WhenServiceNotRegistered_ThrowsInvalidOperationException()
    {
        // Arrange
        var serviceProvider = new ServiceCollection()
            .BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        var controller = new TestController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act & Assert
        // GetRequiredService throws if service is not registered
        Assert.Throws<InvalidOperationException>(() => controller.TestMediator);
    }

    [Fact]
    public void Mediator_WhenCalledMultipleTimes_ReturnsSameInstance()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var serviceProvider = new ServiceCollection()
            .AddSingleton(mockMediator.Object)
            .BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        var controller = new TestController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var mediator1 = controller.TestMediator;
        var mediator2 = controller.TestMediator;

        // Assert
        Assert.NotNull(mediator1);
        Assert.NotNull(mediator2);
        Assert.Same(mediator1, mediator2);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Creates a test controller with configured HttpContext
    /// </summary>
    private TestController CreateController()
    {
        var controller = new TestController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = new ServiceCollection().BuildServiceProvider()
                }
            }
        };

        return controller;
    }

    #endregion
}

