/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TaskForge.Application.Core;
using TaskForge.Application.TaskItems;
using TaskForge.Domain;
using TaskForge.Domain.Enum;
using TaskForge.Persistence;
using Xunit;

namespace Tests.TaskForge.Application.TaskItems;

/// <summary>
/// Unit tests for Delete.Handler
/// </summary>
public class DeleteHandlerTests
{
    #region Helper Methods

    private DataContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new DataContext(options);
    }

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

    private ILogger<Delete.Handler> CreateLogger()
    {
        return new Mock<ILogger<Delete.Handler>>().Object;
    }

    #endregion

    #region Success Tests

    [Fact]
    public async Task Handle_WhenTaskItemExists_ReturnsSuccess()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Delete.Handler(context, logger);
        
        var existingTaskItem = CreateValidTaskItem();
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var command = new Delete.Command
        {
            Id = existingTaskItem.Id
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task Handle_WhenTaskItemExists_RemovesFromDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Delete.Handler(context, logger);
        
        var existingTaskItem = CreateValidTaskItem();
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var command = new Delete.Command
        {
            Id = existingTaskItem.Id
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedItem = await context.TaskItems.FindAsync(existingTaskItem.Id);
        Assert.Null(deletedItem);
    }

    [Fact]
    public async Task Handle_WhenDeletingMultipleItems_DeletesAllSpecified()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Delete.Handler(context, logger);
        
        var taskItem1 = CreateValidTaskItem();
        taskItem1.Id = Guid.NewGuid();
        var taskItem2 = CreateValidTaskItem();
        taskItem2.Id = Guid.NewGuid();
        var taskItem3 = CreateValidTaskItem();
        taskItem3.Id = Guid.NewGuid();

        await context.TaskItems.AddRangeAsync(taskItem1, taskItem2, taskItem3);
        await context.SaveChangesAsync();

        // Act
        await handler.Handle(new Delete.Command { Id = taskItem1.Id }, CancellationToken.None);
        await handler.Handle(new Delete.Command { Id = taskItem2.Id }, CancellationToken.None);

        // Assert
        var count = await context.TaskItems.CountAsync();
        Assert.Equal(1, count);
        var remainingItem = await context.TaskItems.FindAsync(taskItem3.Id);
        Assert.NotNull(remainingItem);
    }

    #endregion

    #region Failure Tests

    [Fact]
    public async Task Handle_WhenTaskItemNotFound_ReturnsFailure()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Delete.Handler(context, logger);
        
        var command = new Delete.Command
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Task item not found", result.Error);
    }

    [Fact]
    public async Task Handle_WhenTaskItemNotFound_DoesNotChangeDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Delete.Handler(context, logger);
        
        var existingTaskItem = CreateValidTaskItem();
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var command = new Delete.Command
        {
            Id = Guid.NewGuid() // Different ID
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        var count = await context.TaskItems.CountAsync();
        Assert.Equal(1, count);
        var remainingItem = await context.TaskItems.FindAsync(existingTaskItem.Id);
        Assert.NotNull(remainingItem);
    }

    [Fact]
    public async Task Handle_WhenCancellationTokenIsCancelled_ThrowsOperationCancelledException()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Delete.Handler(context, logger);
        
        var existingTaskItem = CreateValidTaskItem();
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var command = new Delete.Command
        {
            Id = existingTaskItem.Id
        };

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => 
            handler.Handle(command, cancellationTokenSource.Token));
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task Handle_WhenIdIsEmpty_ReturnsFailure()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Delete.Handler(context, logger);
        
        var command = new Delete.Command
        {
            Id = Guid.Empty
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Task item not found", result.Error);
    }

    [Fact]
    public async Task Handle_WhenDeletingAlreadyDeletedItem_ReturnsFailure()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Delete.Handler(context, logger);
        
        var existingTaskItem = CreateValidTaskItem();
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var command = new Delete.Command
        {
            Id = existingTaskItem.Id
        };

        // First deletion
        await handler.Handle(command, CancellationToken.None);

        // Act - Try to delete again
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Task item not found", result.Error);
    }

    [Fact]
    public async Task Handle_WhenDatabaseIsEmpty_ReturnsFailure()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Delete.Handler(context, logger);
        
        var command = new Delete.Command
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Task item not found", result.Error);
    }

    #endregion

    #region Database State Tests

    [Fact]
    public async Task Handle_WhenDeletingLastItem_DatabaseIsEmpty()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Delete.Handler(context, logger);
        
        var existingTaskItem = CreateValidTaskItem();
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var command = new Delete.Command
        {
            Id = existingTaskItem.Id
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var count = await context.TaskItems.CountAsync();
        Assert.Equal(0, count);
    }

    #endregion
}

