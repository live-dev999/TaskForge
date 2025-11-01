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
/// Unit tests for Create.CommandValidator and Edit.CommandValidator
/// </summary>
public class CommandValidatorTests
{
    #region Create Command Validator Tests

    [Fact]
    public void CreateCommandValidator_WhenTaskItemIsNull_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new Create.CommandValidator();
        var command = new Create.Command
        {
            TaskItem = null
        };

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TaskItem);
    }

    [Fact]
    public void CreateCommandValidator_WhenTaskItemIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var validator = new Create.CommandValidator();
        var command = new Create.Command
        {
            TaskItem = CreateValidTaskItem()
        };

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TaskItem);
    }

    [Fact]
    public void CreateCommandValidator_WhenTaskItemHasInvalidTitle_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new Create.CommandValidator();
        var taskItem = CreateValidTaskItem();
        taskItem.Title = string.Empty;
        var command = new Create.Command
        {
            TaskItem = taskItem
        };

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TaskItem.Title);
    }

    [Fact]
    public void CreateCommandValidator_WhenTaskItemHasInvalidDescription_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new Create.CommandValidator();
        var taskItem = CreateValidTaskItem();
        taskItem.Description = null;
        var command = new Create.Command
        {
            TaskItem = taskItem
        };

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TaskItem.Description);
    }

    #endregion

    #region Edit Command Validator Tests

    [Fact]
    public void EditCommandValidator_WhenTaskItemIsNull_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new Edit.CommandValidator();
        var command = new Edit.Command
        {
            TaskItem = null
        };

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TaskItem);
    }

    [Fact]
    public void EditCommandValidator_WhenTaskItemIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var validator = new Edit.CommandValidator();
        var command = new Edit.Command
        {
            TaskItem = CreateValidTaskItem()
        };

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TaskItem);
    }

    [Fact]
    public void EditCommandValidator_WhenTaskItemHasInvalidTitle_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new Edit.CommandValidator();
        var taskItem = CreateValidTaskItem();
        taskItem.Title = "   ";
        var command = new Edit.Command
        {
            TaskItem = taskItem
        };

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TaskItem.Title);
    }

    [Fact]
    public void EditCommandValidator_WhenTaskItemHasInvalidStatus_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new Edit.CommandValidator();
        var taskItem = CreateValidTaskItem();
        taskItem.Status = default(TaskItemStatus);
        var command = new Edit.Command
        {
            TaskItem = taskItem
        };

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TaskItem.Status);
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

