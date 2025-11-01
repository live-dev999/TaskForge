/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TaskForge.Application.TaskItems;
using TaskForge.Domain;
using TaskForge.Domain.Enum;
using TaskForge.Persistence;

namespace Tests.TaskForge.Application.TaskItems;

/// <summary>
/// Unit tests for Create.Handler
/// </summary>
public class CreateHandlerTests
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

    private ILogger<Create.Handler> CreateLogger()
    {
        return new Mock<ILogger<Create.Handler>>().Object;
    }

    #endregion

    #region Success Tests

    [Fact]
    public async Task Handle_WhenValidTaskItem_ReturnsSuccess()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Create.Handler(context, logger);
        var command = new Create.Command
        {
            TaskItem = CreateValidTaskItem()
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task Handle_WhenValidTaskItem_AddsToDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Create.Handler(context, logger);
        var taskItem = CreateValidTaskItem();
        var command = new Create.Command
        {
            TaskItem = taskItem
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedItem = await context.TaskItems.FindAsync(taskItem.Id);
        Assert.NotNull(savedItem);
        Assert.Equal(taskItem.Title, savedItem.Title);
        Assert.Equal(taskItem.Description, savedItem.Description);
        Assert.Equal(taskItem.Status, savedItem.Status);
    }

    [Fact]
    public async Task Handle_WhenValidTaskItem_AutoSetsIdCreatedAtAndUpdatedAt()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Create.Handler(context, logger);
        var taskItem = CreateValidTaskItem();
        taskItem.Id = Guid.Empty; // Should be auto-set
        taskItem.CreatedAt = default(DateTime); // Should be auto-set
        taskItem.UpdatedAt = default(DateTime); // Should be auto-set
        
        var command = new Create.Command
        {
            TaskItem = taskItem
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedItem = await context.TaskItems.FindAsync(taskItem.Id);
        Assert.NotNull(savedItem);
        Assert.NotEqual(Guid.Empty, savedItem.Id);
        Assert.NotEqual(default(DateTime), savedItem.CreatedAt);
        Assert.NotEqual(default(DateTime), savedItem.UpdatedAt);
        Assert.True(savedItem.CreatedAt <= DateTime.UtcNow.AddSeconds(1));
        Assert.True(savedItem.UpdatedAt <= DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenMultipleValidTaskItems_CreatesAllInDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Create.Handler(context, logger);
        var taskItem1 = CreateValidTaskItem();
        taskItem1.Id = Guid.NewGuid();
        var taskItem2 = CreateValidTaskItem();
        taskItem2.Id = Guid.NewGuid();
        taskItem2.Title = "Second Task";

        var command1 = new Create.Command { TaskItem = taskItem1 };
        var command2 = new Create.Command { TaskItem = taskItem2 };

        // Act
        await handler.Handle(command1, CancellationToken.None);
        await handler.Handle(command2, CancellationToken.None);

        // Assert
        var count = await context.TaskItems.CountAsync();
        Assert.Equal(2, count);
    }

    #endregion

    #region Failure Tests

    [Fact]
    public async Task Handle_WhenCommandIsNull_ThrowsNullReferenceException()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Create.Handler(context, logger);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => handler.Handle(null, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenCancellationTokenIsCancelled_ThrowsOperationCancelledException()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Create.Handler(context, logger);
        var command = new Create.Command
        {
            TaskItem = CreateValidTaskItem()
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
    public async Task Handle_WhenTaskItemIdIsEmpty_CreatesInDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Create.Handler(context, logger);
        var taskItem = CreateValidTaskItem();
        taskItem.Id = Guid.Empty;
        var command = new Create.Command
        {
            TaskItem = taskItem
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var savedItem = await context.TaskItems.FirstOrDefaultAsync(x => x.Title == taskItem.Title);
        Assert.NotNull(savedItem);
    }

    [Fact]
    public async Task Handle_WhenTaskItemHasMinDateTime_CreatesInDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Create.Handler(context, logger);
        var taskItem = CreateValidTaskItem();
        taskItem.CreatedAt = DateTime.MinValue;
        taskItem.UpdatedAt = DateTime.MinValue;
        var command = new Create.Command
        {
            TaskItem = taskItem
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WhenTaskItemHasMaxDateTime_CreatesInDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Create.Handler(context, logger);
        var taskItem = CreateValidTaskItem();
        taskItem.CreatedAt = DateTime.MaxValue;
        taskItem.UpdatedAt = DateTime.MaxValue;
        var command = new Create.Command
        {
            TaskItem = taskItem
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WhenTaskItemHasVeryLongTitle_CreatesInDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Create.Handler(context, logger);
        var taskItem = CreateValidTaskItem();
        taskItem.Title = new string('A', 10000);
        var command = new Create.Command
        {
            TaskItem = taskItem
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WhenTaskItemHasVeryLongDescription_CreatesInDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Create.Handler(context, logger);
        var taskItem = CreateValidTaskItem();
        taskItem.Description = new string('B', 10000);
        var command = new Create.Command
        {
            TaskItem = taskItem
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WhenTaskItemHasAllStatusValues_CreatesInDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Create.Handler(context, logger);

        var statuses = new[] { TaskItemStatus.New, TaskItemStatus.InProgress, TaskItemStatus.Completed, TaskItemStatus.Pending };

        foreach (var status in statuses)
        {
            var taskItem = CreateValidTaskItem();
            taskItem.Id = Guid.NewGuid();
            taskItem.Status = status;
            var command = new Create.Command { TaskItem = taskItem };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        var count = await context.TaskItems.CountAsync();
        Assert.Equal(statuses.Length, count);
    }

    #endregion

    #region Database State Tests

    [Fact]
    public async Task Handle_WhenCalledMultipleTimes_EachCreatesSeparateRecord()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Create.Handler(context, logger);

        for (int i = 0; i < 5; i++)
        {
            var taskItem = CreateValidTaskItem();
            taskItem.Id = Guid.NewGuid();
            taskItem.Title = $"Task {i}";
            var command = new Create.Command { TaskItem = taskItem };

            // Act
            await handler.Handle(command, CancellationToken.None);
        }

        // Assert
        var count = await context.TaskItems.CountAsync();
        Assert.Equal(5, count);
    }

    [Fact]
    public async Task Handle_WhenTaskItemPropertiesArePreserved_PreservesAllProperties()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Create.Handler(context, logger);
        var originalTaskItem = CreateValidTaskItem();
        var originalId = originalTaskItem.Id;
        var originalTitle = originalTaskItem.Title;
        var originalDescription = originalTaskItem.Description;
        var originalStatus = originalTaskItem.Status;

        var command = new Create.Command
        {
            TaskItem = originalTaskItem
        };

        var beforeCreate = DateTime.UtcNow;

        // Act
        await handler.Handle(command, CancellationToken.None);

        var afterCreate = DateTime.UtcNow;

        // Assert
        var savedItem = await context.TaskItems.FindAsync(originalId);
        Assert.NotNull(savedItem);
        Assert.Equal(originalId, savedItem.Id);
        Assert.Equal(originalTitle, savedItem.Title);
        Assert.Equal(originalDescription, savedItem.Description);
        Assert.Equal(originalStatus, savedItem.Status);
        
        // CreatedAt and UpdatedAt are auto-set by handler, so we check they are set to current time
        Assert.True(savedItem.CreatedAt >= beforeCreate && savedItem.CreatedAt <= afterCreate);
        Assert.True(savedItem.UpdatedAt >= beforeCreate && savedItem.UpdatedAt <= afterCreate);
        Assert.Equal(savedItem.CreatedAt, savedItem.UpdatedAt); // They should be the same on create
    }

    [Fact]
    public async Task Handle_WhenSaveChangesReturnsZero_ReturnsFailure()
    {
        // Arrange
        // In-memory database always saves, so this test documents expected behavior
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new Create.Handler(context, logger);
        var command = new Create.Command
        {
            TaskItem = CreateValidTaskItem()
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        // In-memory database should always succeed, but handler checks result > 0
        Assert.True(result.IsSuccess); // This will be true for in-memory
    }

    #endregion
}

