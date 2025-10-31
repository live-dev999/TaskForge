/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using FluentValidation.TestHelper;
using TaskForge.Application.TaskItem;
using TaskForge.Domain;
using TaskForge.Domain.Enum;
using Xunit;

namespace Tests.TaskForge.Application.TaskItem;

/// <summary>
/// Unit tests for TaskItemValidator
/// </summary>
public class TaskItemValidatorTests
{
    private readonly TaskItemValidator _validator;

    public TaskItemValidatorTests()
    {
        _validator = new TaskItemValidator();
    }

    #region Title Validation Tests

    [Fact]
    public void Validate_WhenTitleIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.Title = string.Empty;

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_WhenTitleIsNull_ShouldHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.Title = null;

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_WhenTitleIsWhitespace_ShouldHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.Title = "   ";

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_WhenTitleIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.Title = "Valid Title";

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_WhenTitleIsLong_ShouldNotHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.Title = new string('A', 1000);

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    #endregion

    #region Description Validation Tests

    [Fact]
    public void Validate_WhenDescriptionIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.Description = string.Empty;

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_WhenDescriptionIsNull_ShouldHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.Description = null;

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_WhenDescriptionIsWhitespace_ShouldHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.Description = "   ";

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_WhenDescriptionIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.Description = "Valid Description";

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    #endregion

    #region CreatedAt Validation Tests

    [Fact]
    public void Validate_WhenCreatedAtIsDefault_ShouldHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.CreatedAt = default(DateTime);

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreatedAt);
    }

    [Fact]
    public void Validate_WhenCreatedAtIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.CreatedAt = DateTime.UtcNow;

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CreatedAt);
    }

    [Fact]
    public void Validate_WhenCreatedAtIsInPast_ShouldNotHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.CreatedAt = DateTime.UtcNow.AddDays(-10);

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CreatedAt);
    }

    [Fact]
    public void Validate_WhenCreatedAtIsInFuture_ShouldNotHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.CreatedAt = DateTime.UtcNow.AddDays(10);

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        // Note: Validator only checks for NotEmpty, not if date is in future
        result.ShouldNotHaveValidationErrorFor(x => x.CreatedAt);
    }

    #endregion

    #region UpdatedAt Validation Tests

    [Fact]
    public void Validate_WhenUpdatedAtIsDefault_ShouldHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.UpdatedAt = default(DateTime);

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdatedAt);
    }

    [Fact]
    public void Validate_WhenUpdatedAtIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.UpdatedAt = DateTime.UtcNow;

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UpdatedAt);
    }

    #endregion

    #region Status Validation Tests

    [Fact]
    public void Validate_WhenStatusIsDefault_ShouldHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.Status = default(TaskStatus);

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Validate_WhenStatusIsNew_ShouldNotHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.Status = TaskStatus.New;

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Validate_WhenStatusIsInProgress_ShouldNotHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.Status = TaskStatus.InProgress;

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Validate_WhenStatusIsCompleted_ShouldNotHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.Status = TaskStatus.Completed;

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Validate_WhenStatusIsPending_ShouldNotHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.Status = TaskStatus.Pending;

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    #endregion

    #region Complete Validation Tests

    [Fact]
    public void Validate_WhenAllFieldsAreValid_ShouldNotHaveAnyValidationErrors()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();

        // Act
        var result = _validator.Validate(taskItem);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WhenMultipleFieldsAreInvalid_ShouldHaveMultipleValidationErrors()
    {
        // Arrange
        var taskItem = new Domain.TaskItem
        {
            Title = null,
            Description = null,
            Status = default(TaskStatus),
            CreatedAt = default(DateTime),
            UpdatedAt = default(DateTime)
        };

        // Act
        var result = _validator.Validate(taskItem);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 5);
    }

    #endregion

    #region Helper Methods

    private Domain.TaskItem CreateValidTaskItem()
    {
        return new Domain.TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Title",
            Description = "Test Description",
            Status = TaskStatus.New,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    #endregion
}

