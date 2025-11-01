/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using TaskForge.API.Middleware;
using TaskForge.Application.Core;
using Xunit;

namespace Tests.TaskForge.API;

/// <summary>
/// Unit tests for ExceptionMiddleware
/// </summary>
public class ExceptionMiddlewareTests
{
    #region Helper Methods

    private HttpContext CreateHttpContext()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        return httpContext;
    }

    private ILogger<ExceptionMiddleware> CreateLogger()
    {
        return new Mock<ILogger<ExceptionMiddleware>>().Object;
    }

    private IHostEnvironment CreateDevelopmentEnvironment()
    {
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.Setup(e => e.IsDevelopment()).Returns(true);
        mockEnvironment.Setup(e => e.EnvironmentName).Returns("Development");
        return mockEnvironment.Object;
    }

    private IHostEnvironment CreateProductionEnvironment()
    {
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.Setup(e => e.IsDevelopment()).Returns(false);
        mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
        return mockEnvironment.Object;
    }

    #endregion

    #region Success Tests

    [Fact]
    public async Task InvokeAsync_WhenNoException_InvokesNextAndDoesNotModifyResponse()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var logger = CreateLogger();
        var environment = CreateDevelopmentEnvironment();
        
        var nextCalled = false;
        RequestDelegate next = async (context) =>
        {
            nextCalled = true;
            await Task.CompletedTask;
        };

        var middleware = new ExceptionMiddleware(next, logger, environment);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal(200, httpContext.Response.StatusCode); // Default status code
    }

    #endregion

    #region Exception Handling Tests - Development Environment

    [Fact]
    public async Task InvokeAsync_WhenExceptionInDevelopment_ReturnsDetailedError()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var logger = CreateLogger();
        var environment = CreateDevelopmentEnvironment();
        
        var exceptionMessage = "Test exception message";
        var exceptionStackTrace = "Test stack trace";
        var exception = new Exception(exceptionMessage)
        {
            Data = { ["StackTrace"] = exceptionStackTrace }
        };

        RequestDelegate next = async (context) =>
        {
            throw exception;
        };

        var middleware = new ExceptionMiddleware(next, logger, environment);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, httpContext.Response.StatusCode);
        Assert.Equal("application/json", httpContext.Response.ContentType);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var jsonDocument = JsonDocument.Parse(responseBody);
        
        Assert.Equal((int)HttpStatusCode.InternalServerError, jsonDocument.RootElement.GetProperty("statusCode").GetInt32());
        Assert.Equal(exceptionMessage, jsonDocument.RootElement.GetProperty("message").GetString());
        Assert.True(jsonDocument.RootElement.TryGetProperty("details", out var details));
        Assert.NotNull(details.GetString());
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionInDevelopment_LogsError()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
        var environment = CreateDevelopmentEnvironment();
        
        var exception = new Exception("Test exception");

        RequestDelegate next = async (context) =>
        {
            throw exception;
        };

        var middleware = new ExceptionMiddleware(next, loggerMock.Object, environment);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                exception,
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionWithNullStackTrace_HandlesGracefully()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var logger = CreateLogger();
        var environment = CreateDevelopmentEnvironment();
        
        var exception = new Exception("Test exception");

        RequestDelegate next = async (context) =>
        {
            throw exception;
        };

        var middleware = new ExceptionMiddleware(next, logger, environment);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, httpContext.Response.StatusCode);
        
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var jsonDocument = JsonDocument.Parse(responseBody);
        
        Assert.Equal((int)HttpStatusCode.InternalServerError, jsonDocument.RootElement.GetProperty("statusCode").GetInt32());
        Assert.Equal("Test exception", jsonDocument.RootElement.GetProperty("message").GetString());
    }

    #endregion

    #region Exception Handling Tests - Production Environment

    [Fact]
    public async Task InvokeAsync_WhenExceptionInProduction_ReturnsGenericError()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var logger = CreateLogger();
        var environment = CreateProductionEnvironment();
        
        var exception = new Exception("Test exception message")
        {
            Data = { ["StackTrace"] = "Test stack trace" }
        };

        RequestDelegate next = async (context) =>
        {
            throw exception;
        };

        var middleware = new ExceptionMiddleware(next, logger, environment);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, httpContext.Response.StatusCode);
        Assert.Equal("application/json", httpContext.Response.ContentType);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var jsonDocument = JsonDocument.Parse(responseBody);
        
        Assert.Equal((int)HttpStatusCode.InternalServerError, jsonDocument.RootElement.GetProperty("statusCode").GetInt32());
        Assert.Equal("Internal Server Error", jsonDocument.RootElement.GetProperty("message").GetString());
        Assert.False(jsonDocument.RootElement.TryGetProperty("details", out _)); // No details in production
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionInProduction_DoesNotExposeExceptionDetails()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var logger = CreateLogger();
        var environment = CreateProductionEnvironment();
        
        var sensitiveExceptionMessage = "SQL connection failed: password=secret123";
        var exception = new Exception(sensitiveExceptionMessage);

        RequestDelegate next = async (context) =>
        {
            throw exception;
        };

        var middleware = new ExceptionMiddleware(next, logger, environment);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        
        // Should not contain sensitive exception message
        Assert.DoesNotContain(sensitiveExceptionMessage, responseBody);
        Assert.Contains("Internal Server Error", responseBody);
    }

    #endregion

    #region JSON Serialization Tests

    [Fact]
    public async Task InvokeAsync_WhenException_SerializesResponseAsCamelCase()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var logger = CreateLogger();
        var environment = CreateDevelopmentEnvironment();
        
        var exception = new Exception("Test exception");

        RequestDelegate next = async (context) =>
        {
            throw exception;
        };

        var middleware = new ExceptionMiddleware(next, logger, environment);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var jsonDocument = JsonDocument.Parse(responseBody);
        
        // Properties should be in camelCase
        Assert.True(jsonDocument.RootElement.TryGetProperty("statusCode", out _));
        Assert.True(jsonDocument.RootElement.TryGetProperty("message", out _));
        Assert.True(jsonDocument.RootElement.TryGetProperty("details", out _));
    }

    [Fact]
    public async Task InvokeAsync_WhenException_ResponseIsValidJson()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var logger = CreateLogger();
        var environment = CreateDevelopmentEnvironment();
        
        var exception = new Exception("Test exception");

        RequestDelegate next = async (context) =>
        {
            throw exception;
        };

        var middleware = new ExceptionMiddleware(next, logger, environment);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        
        // Should be valid JSON
        var jsonDocument = JsonDocument.Parse(responseBody);
        Assert.NotNull(jsonDocument);
    }

    #endregion

    #region Different Exception Types Tests

    [Fact]
    public async Task InvokeAsync_WhenArgumentException_HandlesCorrectly()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var logger = CreateLogger();
        var environment = CreateDevelopmentEnvironment();
        
        var exception = new ArgumentException("Invalid argument");

        RequestDelegate next = async (context) =>
        {
            throw exception;
        };

        var middleware = new ExceptionMiddleware(next, logger, environment);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, httpContext.Response.StatusCode);
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var jsonDocument = JsonDocument.Parse(responseBody);
        
        Assert.Equal("Invalid argument", jsonDocument.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public async Task InvokeAsync_WhenNullReferenceException_HandlesCorrectly()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var logger = CreateLogger();
        var environment = CreateDevelopmentEnvironment();
        
        var exception = new NullReferenceException("Object reference not set");

        RequestDelegate next = async (context) =>
        {
            throw exception;
        };

        var middleware = new ExceptionMiddleware(next, logger, environment);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, httpContext.Response.StatusCode);
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var jsonDocument = JsonDocument.Parse(responseBody);
        
        Assert.Equal("Object reference not set", jsonDocument.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public async Task InvokeAsync_WhenInvalidOperationException_HandlesCorrectly()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var logger = CreateLogger();
        var environment = CreateDevelopmentEnvironment();
        
        var exception = new InvalidOperationException("Invalid operation");

        RequestDelegate next = async (context) =>
        {
            throw exception;
        };

        var middleware = new ExceptionMiddleware(next, logger, environment);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, httpContext.Response.StatusCode);
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var jsonDocument = JsonDocument.Parse(responseBody);
        
        Assert.Equal("Invalid operation", jsonDocument.RootElement.GetProperty("message").GetString());
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task InvokeAsync_WhenExceptionWithEmptyMessage_HandlesCorrectly()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var logger = CreateLogger();
        var environment = CreateDevelopmentEnvironment();
        
        var exception = new Exception("");

        RequestDelegate next = async (context) =>
        {
            throw exception;
        };

        var middleware = new ExceptionMiddleware(next, logger, environment);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, httpContext.Response.StatusCode);
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var jsonDocument = JsonDocument.Parse(responseBody);
        
        Assert.Equal("", jsonDocument.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public async Task InvokeAsync_WhenMultipleExceptions_HandlesEachSeparately()
    {
        // Arrange
        var logger = CreateLogger();
        var environment = CreateDevelopmentEnvironment();
        
        var httpContext1 = CreateHttpContext();
        var httpContext2 = CreateHttpContext();
        
        var exception1 = new Exception("First exception");
        var exception2 = new Exception("Second exception");

        RequestDelegate next1 = async (context) => throw exception1;
        RequestDelegate next2 = async (context) => throw exception2;

        var middleware1 = new ExceptionMiddleware(next1, logger, environment);
        var middleware2 = new ExceptionMiddleware(next2, logger, environment);

        // Act
        await middleware1.InvokeAsync(httpContext1);
        await middleware2.InvokeAsync(httpContext2);

        // Assert
        httpContext1.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody1 = await new StreamReader(httpContext1.Response.Body).ReadToEndAsync();
        var jsonDocument1 = JsonDocument.Parse(responseBody1);
        Assert.Equal("First exception", jsonDocument1.RootElement.GetProperty("message").GetString());

        httpContext2.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody2 = await new StreamReader(httpContext2.Response.Body).ReadToEndAsync();
        var jsonDocument2 = JsonDocument.Parse(responseBody2);
        Assert.Equal("Second exception", jsonDocument2.RootElement.GetProperty("message").GetString());
    }

    #endregion
}

