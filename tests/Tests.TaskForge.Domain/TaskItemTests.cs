/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using TaskForge.Domain;
using TaskForge.Domain.Enum;

namespace Tests.TaskForge.Domain;

/// <summary>
/// Unit tests for TaskItem domain model
/// </summary>
public class TaskItemTests
{
    #region Property Tests

    [Fact]
    public void TaskItem_WhenCreated_PropertiesCanBeSet()
    {
        // Arrange
        var taskItem = new TaskItem();

        // Act
        var id = Guid.NewGuid();
        var title = "Test Title";
        var description = "Test Description";
        var status = TaskItemStatus.New;
        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow;

        taskItem.Id = id;
        taskItem.Title = title;
        taskItem.Description = description;
        taskItem.Status = status;
        taskItem.CreatedAt = createdAt;
        taskItem.UpdatedAt = updatedAt;

        // Assert
        Assert.Equal(id, taskItem.Id);
        Assert.Equal(title, taskItem.Title);
        Assert.Equal(description, taskItem.Description);
        Assert.Equal(status, taskItem.Status);
        Assert.Equal(createdAt, taskItem.CreatedAt);
        Assert.Equal(updatedAt, taskItem.UpdatedAt);
    }

    [Fact]
    public void TaskItem_WhenCreated_PropertiesHaveDefaultValues()
    {
        // Arrange & Act
        var taskItem = new TaskItem();

        // Assert
        Assert.Equal(Guid.Empty, taskItem.Id);
        Assert.Null(taskItem.Title);
        Assert.Null(taskItem.Description);
        Assert.Equal(default(TaskItemStatus), taskItem.Status);
        Assert.Equal(default(DateTime), taskItem.CreatedAt);
        Assert.Equal(default(DateTime), taskItem.UpdatedAt);
    }

    #endregion

    #region Id Property Tests

    [Fact]
    public void TaskItem_Id_CanBeSetToNewGuid()
    {
        // Arrange
        var taskItem = new TaskItem();
        var newId = Guid.NewGuid();

        // Act
        taskItem.Id = newId;

        // Assert
        Assert.Equal(newId, taskItem.Id);
    }

    [Fact]
    public void TaskItem_Id_CanBeSetToEmptyGuid()
    {
        // Arrange
        var taskItem = new TaskItem();

        // Act
        taskItem.Id = Guid.Empty;

        // Assert
        Assert.Equal(Guid.Empty, taskItem.Id);
    }

    #endregion

    #region Title Property Tests

    [Fact]
    public void TaskItem_Title_CanBeSetToString()
    {
        // Arrange
        var taskItem = new TaskItem();

        // Act
        taskItem.Title = "Test Title";

        // Assert
        Assert.Equal("Test Title", taskItem.Title);
    }

    [Fact]
    public void TaskItem_Title_CanBeSetToNull()
    {
        // Arrange
        var taskItem = new TaskItem();
        taskItem.Title = "Initial Title";

        // Act
        taskItem.Title = null;

        // Assert
        Assert.Null(taskItem.Title);
    }

    [Fact]
    public void TaskItem_Title_CanBeSetToEmptyString()
    {
        // Arrange
        var taskItem = new TaskItem();

        // Act
        taskItem.Title = string.Empty;

        // Assert
        Assert.Equal(string.Empty, taskItem.Title);
    }

    [Fact]
    public void TaskItem_Title_CanBeSetToVeryLongString()
    {
        // Arrange
        var taskItem = new TaskItem();
        var longTitle = new string('A', 10000);

        // Act
        taskItem.Title = longTitle;

        // Assert
        Assert.Equal(longTitle, taskItem.Title);
    }

    #endregion

    #region Description Property Tests

    [Fact]
    public void TaskItem_Description_CanBeSetToString()
    {
        // Arrange
        var taskItem = new TaskItem();

        // Act
        taskItem.Description = "Test Description";

        // Assert
        Assert.Equal("Test Description", taskItem.Description);
    }

    [Fact]
    public void TaskItem_Description_CanBeSetToNull()
    {
        // Arrange
        var taskItem = new TaskItem();
        taskItem.Description = "Initial Description";

        // Act
        taskItem.Description = null;

        // Assert
        Assert.Null(taskItem.Description);
    }

    [Fact]
    public void TaskItem_Description_CanBeSetToEmptyString()
    {
        // Arrange
        var taskItem = new TaskItem();

        // Act
        taskItem.Description = string.Empty;

        // Assert
        Assert.Equal(string.Empty, taskItem.Description);
    }

    [Fact]
    public void TaskItem_Description_CanBeSetToVeryLongString()
    {
        // Arrange
        var taskItem = new TaskItem();
        var longDescription = new string('B', 50000);

        // Act
        taskItem.Description = longDescription;

        // Assert
        Assert.Equal(longDescription, taskItem.Description);
    }

    #endregion

    #region Status Property Tests

    [Fact]
    public void TaskItem_Status_CanBeSetToNew()
    {
        // Arrange
        var taskItem = new TaskItem();

        // Act
        taskItem.Status = TaskItemStatus.New;

        // Assert
        Assert.Equal(TaskItemStatus.New, taskItem.Status);
    }

    [Fact]
    public void TaskItem_Status_CanBeSetToInProgress()
    {
        // Arrange
        var taskItem = new TaskItem();

        // Act
        taskItem.Status = TaskItemStatus.InProgress;

        // Assert
        Assert.Equal(TaskItemStatus.InProgress, taskItem.Status);
    }

    [Fact]
    public void TaskItem_Status_CanBeSetToCompleted()
    {
        // Arrange
        var taskItem = new TaskItem();

        // Act
        taskItem.Status = TaskItemStatus.Completed;

        // Assert
        Assert.Equal(TaskItemStatus.Completed, taskItem.Status);
    }

    [Fact]
    public void TaskItem_Status_CanBeSetToPending()
    {
        // Arrange
        var taskItem = new TaskItem();

        // Act
        taskItem.Status = TaskItemStatus.Pending;

        // Assert
        Assert.Equal(TaskItemStatus.Pending, taskItem.Status);
    }

    [Fact]
    public void TaskItem_Status_CanBeChanged()
    {
        // Arrange
        var taskItem = new TaskItem();
        taskItem.Status = TaskItemStatus.New;

        // Act
        taskItem.Status = TaskItemStatus.InProgress;
        taskItem.Status = TaskItemStatus.Completed;
        taskItem.Status = TaskItemStatus.Pending;

        // Assert
        Assert.Equal(TaskItemStatus.Pending, taskItem.Status);
    }

    #endregion

    #region CreatedAt Property Tests

    [Fact]
    public void TaskItem_CreatedAt_CanBeSetToUtcNow()
    {
        // Arrange
        var taskItem = new TaskItem();
        var now = DateTime.UtcNow;

        // Act
        taskItem.CreatedAt = now;

        // Assert
        Assert.Equal(now, taskItem.CreatedAt);
    }

    [Fact]
    public void TaskItem_CreatedAt_CanBeSetToMinValue()
    {
        // Arrange
        var taskItem = new TaskItem();

        // Act
        taskItem.CreatedAt = DateTime.MinValue;

        // Assert
        Assert.Equal(DateTime.MinValue, taskItem.CreatedAt);
    }

    [Fact]
    public void TaskItem_CreatedAt_CanBeSetToMaxValue()
    {
        // Arrange
        var taskItem = new TaskItem();

        // Act
        taskItem.CreatedAt = DateTime.MaxValue;

        // Assert
        Assert.Equal(DateTime.MaxValue, taskItem.CreatedAt);
    }

    [Fact]
    public void TaskItem_CreatedAt_CanBeSetToPastDate()
    {
        // Arrange
        var taskItem = new TaskItem();
        var pastDate = DateTime.UtcNow.AddDays(-100);

        // Act
        taskItem.CreatedAt = pastDate;

        // Assert
        Assert.Equal(pastDate, taskItem.CreatedAt);
    }

    [Fact]
    public void TaskItem_CreatedAt_CanBeSetToFutureDate()
    {
        // Arrange
        var taskItem = new TaskItem();
        var futureDate = DateTime.UtcNow.AddDays(100);

        // Act
        taskItem.CreatedAt = futureDate;

        // Assert
        Assert.Equal(futureDate, taskItem.CreatedAt);
    }

    #endregion

    #region UpdatedAt Property Tests

    [Fact]
    public void TaskItem_UpdatedAt_CanBeSetToUtcNow()
    {
        // Arrange
        var taskItem = new TaskItem();
        var now = DateTime.UtcNow;

        // Act
        taskItem.UpdatedAt = now;

        // Assert
        Assert.Equal(now, taskItem.UpdatedAt);
    }

    [Fact]
    public void TaskItem_UpdatedAt_CanBeSetToMinValue()
    {
        // Arrange
        var taskItem = new TaskItem();

        // Act
        taskItem.UpdatedAt = DateTime.MinValue;

        // Assert
        Assert.Equal(DateTime.MinValue, taskItem.UpdatedAt);
    }

    [Fact]
    public void TaskItem_UpdatedAt_CanBeSetToMaxValue()
    {
        // Arrange
        var taskItem = new TaskItem();

        // Act
        taskItem.UpdatedAt = DateTime.MaxValue;

        // Assert
        Assert.Equal(DateTime.MaxValue, taskItem.UpdatedAt);
    }

    [Fact]
    public void TaskItem_UpdatedAt_CanBeSetToPastDate()
    {
        // Arrange
        var taskItem = new TaskItem();
        var pastDate = DateTime.UtcNow.AddDays(-50);

        // Act
        taskItem.UpdatedAt = pastDate;

        // Assert
        Assert.Equal(pastDate, taskItem.UpdatedAt);
    }

    [Fact]
    public void TaskItem_UpdatedAt_CanBeSetToFutureDate()
    {
        // Arrange
        var taskItem = new TaskItem();
        var futureDate = DateTime.UtcNow.AddDays(50);

        // Act
        taskItem.UpdatedAt = futureDate;

        // Assert
        Assert.Equal(futureDate, taskItem.UpdatedAt);
    }

    #endregion

    #region Complete Object Tests

    [Fact]
    public void TaskItem_WhenAllPropertiesAreSet_CreatesCompleteObject()
    {
        // Arrange & Act
        var taskItem = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Complete Task",
            Description = "Complete Description",
            Status = TaskItemStatus.InProgress,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        // Assert
        Assert.NotEqual(Guid.Empty, taskItem.Id);
        Assert.NotNull(taskItem.Title);
        Assert.NotNull(taskItem.Description);
        Assert.NotEqual(default(TaskItemStatus), taskItem.Status);
        Assert.NotEqual(default(DateTime), taskItem.CreatedAt);
        Assert.NotEqual(default(DateTime), taskItem.UpdatedAt);
    }

    [Fact]
    public void TaskItem_WhenUpdatedAtIsBeforeCreatedAt_PropertiesAreSetCorrectly()
    {
        // Arrange
        var taskItem = new TaskItem();
        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow.AddDays(-1);

        // Act
        taskItem.CreatedAt = createdAt;
        taskItem.UpdatedAt = updatedAt;

        // Assert
        // Note: This is a business logic issue, but testing current behavior
        Assert.True(taskItem.UpdatedAt < taskItem.CreatedAt);
    }

    [Fact]
    public void TaskItem_WhenCreatedAndUpdated_CanBeUsedMultipleTimes()
    {
        // Arrange
        var taskItem = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task",
            Description = "Description",
            Status = TaskItemStatus.New,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act - Modify properties
        taskItem.Title = "Updated Title";
        taskItem.Status = TaskItemStatus.InProgress;
        taskItem.UpdatedAt = DateTime.UtcNow;

        // Assert
        Assert.Equal("Updated Title", taskItem.Title);
        Assert.Equal(TaskItemStatus.InProgress, taskItem.Status);
        Assert.NotEqual(DateTime.MinValue, taskItem.UpdatedAt);
    }

    #endregion
}

