/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using TaskForge.Domain.Enum;
using Xunit;

namespace Tests.TaskForge.Domain;

/// <summary>
/// Unit tests for TaskStatus enum
/// </summary>
public class TaskStatusTests
{
    #region Enum Value Tests

    [Fact]
    public void TaskStatus_New_HasCorrectValue()
    {
        // Arrange & Act
        var status = TaskStatus.New;

        // Assert
        Assert.Equal(0, (int)status);
    }

    [Fact]
    public void TaskStatus_InProgress_HasCorrectValue()
    {
        // Arrange & Act
        var status = TaskStatus.InProgress;

        // Assert
        Assert.Equal(1, (int)status);
    }

    [Fact]
    public void TaskStatus_Completed_HasCorrectValue()
    {
        // Arrange & Act
        var status = TaskStatus.Completed;

        // Assert
        Assert.Equal(2, (int)status);
    }

    [Fact]
    public void TaskStatus_Pending_HasCorrectValue()
    {
        // Arrange & Act
        var status = TaskStatus.Pending;

        // Assert
        Assert.Equal(3, (int)status);
    }

    #endregion

    #region Enum Conversion Tests

    [Fact]
    public void TaskStatus_CanBeConvertedToInt()
    {
        // Arrange
        var status = TaskStatus.InProgress;

        // Act
        var intValue = (int)status;

        // Assert
        Assert.Equal(1, intValue);
    }

    [Fact]
    public void TaskStatus_CanBeConvertedFromInt()
    {
        // Arrange
        var intValue = 2;

        // Act
        var status = (TaskStatus)intValue;

        // Assert
        Assert.Equal(TaskStatus.Completed, status);
    }

    [Fact]
    public void TaskStatus_CanBeConvertedFromInvalidInt_ReturnsInvalidEnum()
    {
        // Arrange
        var intValue = 999;

        // Act
        var status = (TaskStatus)intValue;

        // Assert
        // Enum allows any integer value, even if not defined
        Assert.Equal(999, (int)status);
    }

    #endregion

    #region Enum Comparison Tests

    [Fact]
    public void TaskStatus_CanBeCompared()
    {
        // Arrange
        var status1 = TaskStatus.New;
        var status2 = TaskStatus.InProgress;
        var status3 = TaskStatus.New;

        // Assert
        Assert.NotEqual(status1, status2);
        Assert.Equal(status1, status3);
        Assert.True(status1 < status2);
        Assert.True(status2 > status1);
    }

    [Fact]
    public void TaskStatus_CanUseSwitchStatement()
    {
        // Arrange
        var status = TaskStatus.Completed;
        string result = null;

        // Act
        switch (status)
        {
            case TaskStatus.New:
                result = "New";
                break;
            case TaskStatus.InProgress:
                result = "In Progress";
                break;
            case TaskStatus.Completed:
                result = "Completed";
                break;
            case TaskStatus.Pending:
                result = "Pending";
                break;
        }

        // Assert
        Assert.Equal("Completed", result);
    }

    #endregion

    #region Enum All Values Tests

    [Fact]
    public void TaskStatus_AllValuesAreDefined()
    {
        // Arrange & Act
        var values = Enum.GetValues<TaskStatus>();

        // Assert
        Assert.Contains(TaskStatus.New, values);
        Assert.Contains(TaskStatus.InProgress, values);
        Assert.Contains(TaskStatus.Completed, values);
        Assert.Contains(TaskStatus.Pending, values);
        Assert.Equal(4, values.Length);
    }

    [Fact]
    public void TaskStatus_CanIterateAllValues()
    {
        // Arrange
        var expectedValues = new[]
        {
            TaskStatus.New,
            TaskStatus.InProgress,
            TaskStatus.Completed,
            TaskStatus.Pending
        };

        // Act
        var actualValues = Enum.GetValues<TaskStatus>();

        // Assert
        Assert.Equal(expectedValues.Length, actualValues.Length);
        foreach (var expectedValue in expectedValues)
        {
            Assert.Contains(expectedValue, actualValues);
        }
    }

    #endregion

    #region Enum String Tests

    [Fact]
    public void TaskStatus_CanBeConvertedToString()
    {
        // Arrange
        var status = TaskStatus.InProgress;

        // Act
        var stringValue = status.ToString();

        // Assert
        Assert.Equal("InProgress", stringValue);
    }

    [Fact]
    public void TaskStatus_CanBeParsedFromString()
    {
        // Arrange
        var stringValue = "Completed";

        // Act
        var status = Enum.Parse<TaskStatus>(stringValue);

        // Assert
        Assert.Equal(TaskStatus.Completed, status);
    }

    [Fact]
    public void TaskStatus_CanBeParsedFromStringCaseInsensitive()
    {
        // Arrange
        var stringValue = "new";

        // Act
        var status = Enum.Parse<TaskStatus>(stringValue, ignoreCase: true);

        // Assert
        Assert.Equal(TaskStatus.New, status);
    }

    [Fact]
    public void TaskStatus_WhenParsingInvalidString_ThrowsArgumentException()
    {
        // Arrange
        var stringValue = "InvalidStatus";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Enum.Parse<TaskStatus>(stringValue));
    }

    #endregion
}

