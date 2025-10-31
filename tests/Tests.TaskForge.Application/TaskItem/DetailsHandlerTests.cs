/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using Application.TaskItems;
using Microsoft.EntityFrameworkCore;
using TaskForge.Application.Core;
using TaskForge.Domain;
using TaskForge.Domain.Enum;
using TaskForge.Persistence;
using Xunit;

namespace Tests.TaskForge.Application.TaskItem;

/// <summary>
/// Unit tests for Details.Handler
/// </summary>
public class DetailsHandlerTests
{
    #region Helper Methods

    private DataContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new DataContext(options);
    }

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

    #region Success Tests

    [Fact]
    public async Task Handle_WhenTaskItemExists_ReturnsSuccess()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var handler = new Details.Handler(context);
        
        var existingTaskItem = CreateValidTaskItem();
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var query = new Details.Query
        {
            Id = existingTaskItem.Id
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(existingTaskItem.Id, result.Value.Id);
    }

    [Fact]
    public async Task Handle_WhenTaskItemExists_ReturnsCorrectTaskItem()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var handler = new Details.Handler(context);
        
        var existingTaskItem = CreateValidTaskItem();
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var query = new Details.Query
        {
            Id = existingTaskItem.Id
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(existingTaskItem.Id, result.Value.Id);
        Assert.Equal(existingTaskItem.Title, result.Value.Title);
        Assert.Equal(existingTaskItem.Description, result.Value.Description);
        Assert.Equal(existingTaskItem.Status, result.Value.Status);
        Assert.Equal(existingTaskItem.CreatedAt, result.Value.CreatedAt);
        Assert.Equal(existingTaskItem.UpdatedAt, result.Value.UpdatedAt);
    }

    [Fact]
    public async Task Handle_WhenMultipleTaskItemsExist_ReturnsCorrectOne()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var handler = new Details.Handler(context);
        
        var taskItem1 = CreateValidTaskItem();
        taskItem1.Id = Guid.NewGuid();
        taskItem1.Title = "First Task";
        
        var taskItem2 = CreateValidTaskItem();
        taskItem2.Id = Guid.NewGuid();
        taskItem2.Title = "Second Task";

        await context.TaskItems.AddRangeAsync(taskItem1, taskItem2);
        await context.SaveChangesAsync();

        var query = new Details.Query
        {
            Id = taskItem2.Id
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Second Task", result.Value.Title);
        Assert.Equal(taskItem2.Id, result.Value.Id);
    }

    #endregion

    #region Failure Tests

    [Fact]
    public async Task Handle_WhenTaskItemNotFound_ReturnsSuccessWithNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var handler = new Details.Handler(context);
        
        var query = new Details.Query
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        // Note: Current implementation returns Success(null), which is a design issue
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_WhenCancellationTokenIsCancelled_ThrowsOperationCancelledException()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var handler = new Details.Handler(context);
        
        var existingTaskItem = CreateValidTaskItem();
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var query = new Details.Query
        {
            Id = existingTaskItem.Id
        };

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => 
            handler.Handle(query, cancellationTokenSource.Token));
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task Handle_WhenIdIsEmpty_ReturnsSuccessWithNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var handler = new Details.Handler(context);
        
        var query = new Details.Query
        {
            Id = Guid.Empty
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_WhenDatabaseIsEmpty_ReturnsSuccessWithNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var handler = new Details.Handler(context);
        
        var query = new Details.Query
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_WhenQueryIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var handler = new Details.Handler(context);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            handler.Handle(null, CancellationToken.None));
    }

    #endregion

    #region Property Preservation Tests

    [Fact]
    public async Task Handle_WhenTaskItemHasAllProperties_PreservesAllProperties()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var handler = new Details.Handler(context);
        
        var existingTaskItem = CreateValidTaskItem();
        existingTaskItem.Status = TaskStatus.InProgress;
        existingTaskItem.CreatedAt = DateTime.UtcNow.AddDays(-10);
        existingTaskItem.UpdatedAt = DateTime.UtcNow.AddDays(-5);
        
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var query = new Details.Query
        {
            Id = existingTaskItem.Id
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(existingTaskItem.Status, result.Value.Status);
        Assert.Equal(existingTaskItem.CreatedAt, result.Value.CreatedAt);
        Assert.Equal(existingTaskItem.UpdatedAt, result.Value.UpdatedAt);
    }

    #endregion
}

