/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using TaskForge.Domain.Enum;

namespace Tests.TaskForge.Domain;

/// <summary>
/// Unit tests for TaskItemStatus enum
/// </summary>
public class TaskItemStatusTests
{
    #region Enum Value Tests

    [Fact]
    public void TaskItemStatus_New_HasCorrectValue()
    {
        // Arrange & Act
        var status = TaskItemStatus.New;

        // Assert
        Assert.Equal(0, (int)status);
    }

    [Fact]
    public void TaskItemStatus_InProgress_HasCorrectValue()
    {
        // Arrange & Act
        var status = TaskItemStatus.InProgress;

        // Assert
        Assert.Equal(1, (int)status);
    }

    [Fact]
    public void TaskItemStatus_Completed_HasCorrectValue()
    {
        // Arrange & Act
        var status = TaskItemStatus.Completed;

        // Assert
        Assert.Equal(2, (int)status);
    }

    [Fact]
    public void TaskItemStatus_Pending_HasCorrectValue()
    {
        // Arrange & Act
        var status = TaskItemStatus.Pending;

        // Assert
        Assert.Equal(3, (int)status);
    }

    #endregion

    #region Enum Conversion Tests

    [Fact]
    public void TaskItemStatus_CanBeConvertedToInt()
    {
        // Arrange
        var status = TaskItemStatus.InProgress;

        // Act
        var intValue = (int)status;

        // Assert
        Assert.Equal(1, intValue);
    }

    [Fact]
    public void TaskItemStatus_CanBeConvertedFromInt()
    {
        // Arrange
        var intValue = 2;

        // Act
        var status = (TaskItemStatus)intValue;

        // Assert
        Assert.Equal(TaskItemStatus.Completed, status);
    }

    [Fact]
    public void TaskItemStatus_CanBeConvertedFromInvalidInt_ReturnsInvalidEnum()
    {
        // Arrange
        var intValue = 999;

        // Act
        var status = (TaskItemStatus)intValue;

        // Assert
        // Enum allows any integer value, even if not defined
        Assert.Equal(999, (int)status);
    }

    #endregion

    #region Enum Comparison Tests

    [Fact]
    public void TaskItemStatus_CanBeCompared()
    {
        // Arrange
        var status1 = TaskItemStatus.New;
        var status2 = TaskItemStatus.InProgress;
        var status3 = TaskItemStatus.New;

        // Assert
        Assert.NotEqual(status1, status2);
        Assert.Equal(status1, status3);
        Assert.True(status1 < status2);
        Assert.True(status2 > status1);
    }

    [Fact]
    public void TaskItemStatus_CanUseSwitchStatement()
    {
        // Arrange
        var status = TaskItemStatus.Completed;
        string result = null;

        // Act
        switch (status)
        {
            case TaskItemStatus.New:
                result = "New";
                break;
            case TaskItemStatus.InProgress:
                result = "In Progress";
                break;
            case TaskItemStatus.Completed:
                result = "Completed";
                break;
            case TaskItemStatus.Pending:
                result = "Pending";
                break;
        }

        // Assert
        Assert.Equal("Completed", result);
    }

    #endregion

    #region Enum All Values Tests

    [Fact]
    public void TaskItemStatus_AllValuesAreDefined()
    {
        // Arrange & Act
        var values = Enum.GetValues<TaskItemStatus>();

        // Assert
        Assert.Contains(TaskItemStatus.New, values);
        Assert.Contains(TaskItemStatus.InProgress, values);
        Assert.Contains(TaskItemStatus.Completed, values);
        Assert.Contains(TaskItemStatus.Pending, values);
        Assert.Equal(4, values.Length);
    }

    [Fact]
    public void TaskItemStatus_CanIterateAllValues()
    {
        // Arrange
        var expectedValues = new[]
        {
            TaskItemStatus.New,
            TaskItemStatus.InProgress,
            TaskItemStatus.Completed,
            TaskItemStatus.Pending
        };

        // Act
        var actualValues = Enum.GetValues<TaskItemStatus>();

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
    public void TaskItemStatus_CanBeConvertedToString()
    {
        // Arrange
        var status = TaskItemStatus.InProgress;

        // Act
        var stringValue = status.ToString();

        // Assert
        Assert.Equal("InProgress", stringValue);
    }

    [Fact]
    public void TaskItemStatus_CanBeParsedFromString()
    {
        // Arrange
        var stringValue = "Completed";

        // Act
        var status = Enum.Parse<TaskItemStatus>(stringValue);

        // Assert
        Assert.Equal(TaskItemStatus.Completed, status);
    }

    [Fact]
    public void TaskItemStatus_CanBeParsedFromStringCaseInsensitive()
    {
        // Arrange
        var stringValue = "new";

        // Act
        var status = Enum.Parse<TaskItemStatus>(stringValue, ignoreCase: true);

        // Assert
        Assert.Equal(TaskItemStatus.New, status);
    }

    [Fact]
    public void TaskItemStatus_WhenParsingInvalidString_ThrowsArgumentException()
    {
        // Arrange
        var stringValue = "InvalidStatus";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Enum.Parse<TaskItemStatus>(stringValue));
    }

    #endregion
}

