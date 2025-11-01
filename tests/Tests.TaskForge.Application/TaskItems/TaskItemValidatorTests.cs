/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using FluentValidation.TestHelper;
using TaskForge.Application.TaskItems;
using TaskForge.Domain;
using TaskForge.Domain.Enum;

namespace Tests.TaskForge.Application.TaskItems;

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
    public void Validate_WhenCreatedAtIsDefault_ShouldNotHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.CreatedAt = default(DateTime);

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        // CreatedAt is set automatically, no validation needed
        result.ShouldNotHaveValidationErrorFor(x => x.CreatedAt);
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

    #endregion

    #region UpdatedAt Validation Tests

    [Fact]
    public void Validate_WhenUpdatedAtIsDefault_ShouldNotHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.UpdatedAt = default(DateTime);

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        // UpdatedAt is set automatically, no validation needed
        result.ShouldNotHaveValidationErrorFor(x => x.UpdatedAt);
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
    public void Validate_WhenStatusIsDefault_ShouldNotHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.Status = default(TaskItemStatus); // default = 0 = New

        // Act
        var result = _validator.TestValidate(taskItem);

        // Assert
        // Status uses IsInEnum, so default(0) = New is actually valid
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Validate_WhenStatusIsInvalidEnumValue_ShouldHaveValidationError()
    {
        // Arrange
        var taskItem = CreateValidTaskItem();
        taskItem.Status = (TaskItemStatus)999; // Invalid enum value

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
        taskItem.Status = TaskItemStatus.New;

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
        taskItem.Status = TaskItemStatus.InProgress;

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
        taskItem.Status = TaskItemStatus.Completed;

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
        taskItem.Status = TaskItemStatus.Pending;

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
        var taskItem = new TaskItem
        {
            Title = null,
            Description = null,
            Status = (TaskItemStatus)999, // Invalid enum value
            CreatedAt = default(DateTime),
            UpdatedAt = default(DateTime)
        };

        // Act
        var result = _validator.Validate(taskItem);

        // Assert
        Assert.False(result.IsValid);
        // Title, Description, Status should have errors (CreatedAt and UpdatedAt are not validated)
        Assert.True(result.Errors.Count >= 3);
    }

    #endregion

    #region Helper Methods

    private TaskItem CreateValidTaskItem()
    {
        return new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Title",
            Description = "Test Description",
            Status = TaskItemStatus.New,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    #endregion
}

