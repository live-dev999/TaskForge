/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using TaskForge.Application.Core;
using Xunit;

namespace Tests.TaskForge.Application;

/// <summary>
/// Unit tests for Result class
/// </summary>
public class ResultTests
{
    #region Success Tests

    [Fact]
    public void Success_WhenCalledWithValue_ReturnsResultWithIsSuccessTrue()
    {
        // Arrange
        var testValue = "test value";

        // Act
        var result = Result<string>.Success(testValue);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(testValue, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Success_WhenCalledWithNull_ReturnsResultWithIsSuccessTrue()
    {
        // Act
        var result = Result<string>.Success(null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Success_WhenCalledWithObject_ReturnsResultWithIsSuccessTrue()
    {
        // Arrange
        var testObject = new { Id = Guid.NewGuid(), Name = "Test" };

        // Act
        var result = Result<object>.Success(testObject);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(testObject, result.Value);
    }

    [Fact]
    public void Success_WhenCalledWithInt_ReturnsResultWithIsSuccessTrue()
    {
        // Arrange
        var testValue = 42;

        // Act
        var result = Result<int>.Success(testValue);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(testValue, result.Value);
    }

    [Fact]
    public void Success_WhenCalledWithZero_ReturnsResultWithIsSuccessTrue()
    {
        // Act
        var result = Result<int>.Success(0);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public void Success_WhenCalledWithGuid_ReturnsResultWithIsSuccessTrue()
    {
        // Arrange
        var testGuid = Guid.NewGuid();

        // Act
        var result = Result<Guid>.Success(testGuid);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(testGuid, result.Value);
    }

    [Fact]
    public void Success_WhenCalledWithEmptyGuid_ReturnsResultWithIsSuccessTrue()
    {
        // Act
        var result = Result<Guid>.Success(Guid.Empty);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Guid.Empty, result.Value);
    }

    [Fact]
    public void Success_WhenCalledWithList_ReturnsResultWithIsSuccessTrue()
    {
        // Arrange
        var testList = new List<string> { "item1", "item2" };

        // Act
        var result = Result<List<string>>.Success(testList);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(testList, result.Value);
    }

    [Fact]
    public void Success_WhenCalledWithEmptyList_ReturnsResultWithIsSuccessTrue()
    {
        // Arrange
        var emptyList = new List<string>();

        // Act
        var result = Result<List<string>>.Success(emptyList);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(emptyList, result.Value);
    }

    #endregion

    #region Failure Tests

    [Fact]
    public void Failure_WhenCalledWithErrorMessage_ReturnsResultWithIsSuccessFalse()
    {
        // Arrange
        var errorMessage = "Something went wrong";

        // Act
        var result = Result<string>.Failure(errorMessage);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public void Failure_WhenCalledWithEmptyErrorMessage_ReturnsResultWithIsSuccessFalse()
    {
        // Act
        var result = Result<string>.Failure("");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("", result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public void Failure_WhenCalledWithNullErrorMessage_ReturnsResultWithIsSuccessFalse()
    {
        // Act
        var result = Result<string>.Failure(null);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public void Failure_WhenCalledWithLongErrorMessage_ReturnsResultWithIsSuccessFalse()
    {
        // Arrange
        var longError = new string('A', 1000);

        // Act
        var result = Result<string>.Failure(longError);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(longError, result.Error);
    }

    #endregion

    #region Property Tests

    [Fact]
    public void IsSuccess_CanBeSetDirectly()
    {
        // Arrange
        var result = new Result<string>
        {
            IsSuccess = true
        };

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Value_CanBeSetDirectly()
    {
        // Arrange
        var testValue = "test";
        var result = new Result<string>
        {
            Value = testValue
        };

        // Assert
        Assert.Equal(testValue, result.Value);
    }

    [Fact]
    public void Error_CanBeSetDirectly()
    {
        // Arrange
        var errorMessage = "Error";
        var result = new Result<string>
        {
            Error = errorMessage
        };

        // Assert
        Assert.Equal(errorMessage, result.Error);
    }

    [Fact]
    public void Result_CanHaveBothSuccessAndError_WhenSetManually()
    {
        // Arrange
        var result = new Result<string>
        {
            IsSuccess = true,
            Value = "value",
            Error = "error"
        };

        // Assert - This is a potential issue in the design, but testing current behavior
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotNull(result.Error);
    }

    #endregion
}

