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
/// Unit tests for List.Handler
/// </summary>
public class ListHandlerTests
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

    private ILogger<List.Handler> CreateLogger()
    {
        return new Mock<ILogger<List.Handler>>().Object;
    }

    #endregion

    #region Success Tests

    [Fact]
    public async Task Handle_WhenNoTaskItems_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new List.Handler(context, logger);
        
        var query = new List.Query();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_WhenOneTaskItemExists_ReturnsListWithOneItem()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new List.Handler(context, logger);
        
        var existingTaskItem = CreateValidTaskItem();
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var query = new List.Query();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        Assert.Equal(existingTaskItem.Id, result.Value[0].Id);
    }

    [Fact]
    public async Task Handle_WhenMultipleTaskItemsExist_ReturnsAllItems()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new List.Handler(context, logger);
        
        var taskItem1 = CreateValidTaskItem();
        taskItem1.Id = Guid.NewGuid();
        taskItem1.Title = "First Task";
        
        var taskItem2 = CreateValidTaskItem();
        taskItem2.Id = Guid.NewGuid();
        taskItem2.Title = "Second Task";
        
        var taskItem3 = CreateValidTaskItem();
        taskItem3.Id = Guid.NewGuid();
        taskItem3.Title = "Third Task";

        await context.TaskItems.AddRangeAsync(taskItem1, taskItem2, taskItem3);
        await context.SaveChangesAsync();

        var query = new List.Query();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.Count);
    }

    [Fact]
    public async Task Handle_WhenMultipleTaskItemsExist_ReturnsAllProperties()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new List.Handler(context, logger);
        
        var taskItem1 = CreateValidTaskItem();
        taskItem1.Id = Guid.NewGuid();
        
        var taskItem2 = CreateValidTaskItem();
        taskItem2.Id = Guid.NewGuid();
        taskItem2.Status = TaskItemStatus.InProgress;

        await context.TaskItems.AddRangeAsync(taskItem1, taskItem2);
        await context.SaveChangesAsync();

        var query = new List.Query();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Value.Count);
        Assert.Contains(result.Value, x => x.Id == taskItem1.Id && x.Status == TaskItemStatus.New);
        Assert.Contains(result.Value, x => x.Id == taskItem2.Id && x.Status == TaskItemStatus.InProgress);
    }

    #endregion

    #region Failure Tests

    [Fact]
    public async Task Handle_WhenCancellationTokenIsCancelled_ThrowsOperationCancelledException()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new List.Handler(context, logger);
        
        var existingTaskItem = CreateValidTaskItem();
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var query = new List.Query();
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => 
            handler.Handle(query, cancellationTokenSource.Token));
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task Handle_WhenQueryIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new List.Handler(context, logger);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            handler.Handle(null, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenManyTaskItemsExist_ReturnsAllItems()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new List.Handler(context, logger);
        
        var taskItems = new List<TaskItem>();
        for (int i = 0; i < 100; i++)
        {
            var taskItem = CreateValidTaskItem();
            taskItem.Id = Guid.NewGuid();
            taskItem.Title = $"Task {i}";
            taskItems.Add(taskItem);
        }

        await context.TaskItems.AddRangeAsync(taskItems);
        await context.SaveChangesAsync();

        var query = new List.Query();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(100, result.Value.Count);
    }

    [Fact]
    public async Task Handle_WhenTaskItemsHaveDifferentStatuses_ReturnsAllWithCorrectStatuses()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new List.Handler(context, logger);
        
        var statuses = new[] { TaskItemStatus.New, TaskItemStatus.InProgress, TaskItemStatus.Completed, TaskItemStatus.Pending };
        var taskItems = new List<TaskItem>();

        for (int i = 0; i < statuses.Length; i++)
        {
            var taskItem = CreateValidTaskItem();
            taskItem.Id = Guid.NewGuid();
            taskItem.Status = statuses[i];
            taskItems.Add(taskItem);
        }

        await context.TaskItems.AddRangeAsync(taskItems);
        await context.SaveChangesAsync();

        var query = new List.Query();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(statuses.Length, result.Value.Count);
        foreach (var status in statuses)
        {
            Assert.Contains(result.Value, x => x.Status == status);
        }
    }

    #endregion

    #region Order Tests

    [Fact]
    public async Task Handle_WhenMultipleTaskItemsExist_OrderMayVary()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new List.Handler(context, logger);
        
        var taskItem1 = CreateValidTaskItem();
        taskItem1.Id = Guid.NewGuid();
        taskItem1.CreatedAt = DateTime.UtcNow.AddDays(-2);
        
        var taskItem2 = CreateValidTaskItem();
        taskItem2.Id = Guid.NewGuid();
        taskItem2.CreatedAt = DateTime.UtcNow.AddDays(-1);

        await context.TaskItems.AddRangeAsync(taskItem1, taskItem2);
        await context.SaveChangesAsync();

        var query = new List.Query();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        // Note: ToListAsync doesn't guarantee order unless explicitly ordered
        Assert.Equal(2, result.Value.Count);
        Assert.Contains(result.Value, x => x.Id == taskItem1.Id);
        Assert.Contains(result.Value, x => x.Id == taskItem2.Id);
    }

    #endregion
}

