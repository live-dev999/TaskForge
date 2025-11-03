/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TaskForge.Application.Core;
using TaskForge.Application.TaskItems;
using TaskForge.Domain;
using TaskForge.Domain.Enum;
using TaskForge.Persistence;

namespace Tests.TaskForge.Application.TaskItems;

/// <summary>
/// Unit tests for Edit.Handler
/// </summary>
public class EditHandlerTests
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

    private IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfiles>();
        });
        return configuration.CreateMapper();
    }

    private ILogger<Edit.Handler> CreateLogger()
    {
        return new Mock<ILogger<Edit.Handler>>().Object;
    }

    private IEventService CreateEventService()
    {
        var mock = new Mock<IEventService>();
        mock.Setup(x => x.SendEventAsync(It.IsAny<TaskChangeEventDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        return mock.Object;
    }

    private IMessageProducer CreateMessageProducer()
    {
        var mock = new Mock<IMessageProducer>();
        mock.Setup(x => x.PublishEventAsync(It.IsAny<TaskChangeEventDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        return mock.Object;
    }

    #endregion

    #region Success Tests

    [Fact]
    public async Task Handle_WhenTaskItemExists_ReturnsSuccess()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mapper = CreateMapper();
        var logger = CreateLogger();
        var eventService = CreateEventService();
        var messageProducer = CreateMessageProducer();
        var handler = new Edit.Handler(context, mapper, logger, eventService, messageProducer);
        
        var existingTaskItem = CreateValidTaskItem();
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var updatedTaskItem = CreateValidTaskItem();
        updatedTaskItem.Id = existingTaskItem.Id;
        updatedTaskItem.Title = "Updated Title";

        var command = new Edit.Command
        {
            TaskItem = updatedTaskItem
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        // Unit is a value type, no need to check for null
    }

    [Fact]
    public async Task Handle_WhenTaskItemExists_UpdatesProperties()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mapper = CreateMapper();
        var logger = CreateLogger();
        var eventService = CreateEventService();
        var messageProducer = CreateMessageProducer();
        var handler = new Edit.Handler(context, mapper, logger, eventService, messageProducer);
        
        var existingTaskItem = CreateValidTaskItem();
        var originalTitle = existingTaskItem.Title;
        var originalDescription = existingTaskItem.Description;
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var updatedTaskItem = CreateValidTaskItem();
        updatedTaskItem.Id = existingTaskItem.Id;
        updatedTaskItem.Title = "New Title";
        updatedTaskItem.Description = "New Description";
        updatedTaskItem.Status = TaskItemStatus.InProgress;

        var command = new Edit.Command
        {
            TaskItem = updatedTaskItem
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedItem = await context.TaskItems.FindAsync(existingTaskItem.Id);
        Assert.NotNull(savedItem);
        Assert.Equal("New Title", savedItem.Title);
        Assert.Equal("New Description", savedItem.Description);
        Assert.Equal(TaskItemStatus.InProgress, savedItem.Status);
        Assert.NotEqual(originalTitle, savedItem.Title);
    }

    [Fact]
    public async Task Handle_WhenTaskItemExists_PreservesCreatedAtAndUpdatesUpdatedAt()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mapper = CreateMapper();
        var logger = CreateLogger();
        var eventService = CreateEventService();
        var messageProducer = CreateMessageProducer();
        var handler = new Edit.Handler(context, mapper, logger, eventService, messageProducer);
        
        var existingTaskItem = CreateValidTaskItem();
        var originalCreatedAt = DateTime.UtcNow.AddDays(-10);
        existingTaskItem.CreatedAt = originalCreatedAt;
        var originalUpdatedAt = DateTime.UtcNow.AddMinutes(-5);
        existingTaskItem.UpdatedAt = originalUpdatedAt;
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        // Reload from database to get actual values after save
        var existingItemFromDb = await context.TaskItems.FindAsync(existingTaskItem.Id);
        var savedCreatedAt = existingItemFromDb.CreatedAt;
        var savedUpdatedAt = existingItemFromDb.UpdatedAt;

        var updatedTaskItem = CreateValidTaskItem();
        updatedTaskItem.Id = existingTaskItem.Id;
        updatedTaskItem.Title = "Updated Title";
        updatedTaskItem.CreatedAt = DateTime.UtcNow.AddDays(10); // This should be ignored

        var command = new Edit.Command
        {
            TaskItem = updatedTaskItem
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedItem = await context.TaskItems.FindAsync(existingTaskItem.Id);
        Assert.NotNull(savedItem);
        Assert.Equal("Updated Title", savedItem.Title);
        Assert.Equal(savedCreatedAt, savedItem.CreatedAt); // CreatedAt should be preserved
        Assert.True(savedItem.UpdatedAt > savedUpdatedAt); // UpdatedAt should be updated
    }

    #endregion

    #region Failure Tests

    [Fact]
    public async Task Handle_WhenTaskItemNotFound_ReturnsFailure()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mapper = CreateMapper();
        var logger = CreateLogger();
        var eventService = CreateEventService();
        var messageProducer = CreateMessageProducer();
        var handler = new Edit.Handler(context, mapper, logger, eventService, messageProducer);
        
        var nonExistentTaskItem = CreateValidTaskItem();
        nonExistentTaskItem.Id = Guid.NewGuid();

        var command = new Edit.Command
        {
            TaskItem = nonExistentTaskItem
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Task item not found", result.Error);
    }

    [Fact]
    public async Task Handle_WhenTaskItemNotFound_DoesNotAddNewItem()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mapper = CreateMapper();
        var logger = CreateLogger();
        var eventService = CreateEventService();
        var messageProducer = CreateMessageProducer();
        var handler = new Edit.Handler(context, mapper, logger, eventService, messageProducer);
        
        var nonExistentTaskItem = CreateValidTaskItem();
        nonExistentTaskItem.Id = Guid.NewGuid();

        var command = new Edit.Command
        {
            TaskItem = nonExistentTaskItem
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        var count = await context.TaskItems.CountAsync();
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task Handle_WhenCancellationTokenIsCancelled_ThrowsOperationCancelledException()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mapper = CreateMapper();
        var logger = CreateLogger();
        var eventService = CreateEventService();
        var messageProducer = CreateMessageProducer();
        var handler = new Edit.Handler(context, mapper, logger, eventService, messageProducer);
        
        var existingTaskItem = CreateValidTaskItem();
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var updatedTaskItem = CreateValidTaskItem();
        updatedTaskItem.Id = existingTaskItem.Id;

        var command = new Edit.Command
        {
            TaskItem = updatedTaskItem
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
    public async Task Handle_WhenTaskItemIdIsEmpty_ReturnsFailure()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mapper = CreateMapper();
        var logger = CreateLogger();
        var eventService = CreateEventService();
        var messageProducer = CreateMessageProducer();
        var handler = new Edit.Handler(context, mapper, logger, eventService, messageProducer);
        
        var taskItem = CreateValidTaskItem();
        taskItem.Id = Guid.Empty;

        var command = new Edit.Command
        {
            TaskItem = taskItem
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Task item not found", result.Error);
    }

    [Fact]
    public async Task Handle_WhenUpdatingWithSameValues_ReturnsSuccess()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mapper = CreateMapper();
        var logger = CreateLogger();
        var eventService = CreateEventService();
        var messageProducer = CreateMessageProducer();
        var handler = new Edit.Handler(context, mapper, logger, eventService, messageProducer);
        
        var existingTaskItem = CreateValidTaskItem();
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var sameTaskItem = CreateValidTaskItem();
        sameTaskItem.Id = existingTaskItem.Id;
        sameTaskItem.Title = existingTaskItem.Title;
        sameTaskItem.Description = existingTaskItem.Description;

        var command = new Edit.Command
        {
            TaskItem = sameTaskItem
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WhenUpdatingAllStatusValues_UpdatesCorrectly()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mapper = CreateMapper();
        var logger = CreateLogger();
        var eventService = CreateEventService();
        var messageProducer = CreateMessageProducer();
        var handler = new Edit.Handler(context, mapper, logger, eventService, messageProducer);
        
        var existingTaskItem = CreateValidTaskItem();
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var statuses = new[] { TaskItemStatus.New, TaskItemStatus.InProgress, TaskItemStatus.Completed, TaskItemStatus.Pending };

        foreach (var status in statuses)
        {
            var updatedTaskItem = CreateValidTaskItem();
            updatedTaskItem.Id = existingTaskItem.Id;
            updatedTaskItem.Status = status;

            var command = new Edit.Command
            {
                TaskItem = updatedTaskItem
            };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var savedItem = await context.TaskItems.FindAsync(existingTaskItem.Id);
            Assert.Equal(status, savedItem.Status);
        }
    }

    [Fact]
    public async Task Handle_WhenUpdatingWithVeryLongTitle_UpdatesCorrectly()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mapper = CreateMapper();
        var logger = CreateLogger();
        var eventService = CreateEventService();
        var messageProducer = CreateMessageProducer();
        var handler = new Edit.Handler(context, mapper, logger, eventService, messageProducer);
        
        var existingTaskItem = CreateValidTaskItem();
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        var updatedTaskItem = CreateValidTaskItem();
        updatedTaskItem.Id = existingTaskItem.Id;
        updatedTaskItem.Title = new string('A', 10000);

        var command = new Edit.Command
        {
            TaskItem = updatedTaskItem
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var savedItem = await context.TaskItems.FindAsync(existingTaskItem.Id);
        Assert.Equal(new string('A', 10000), savedItem.Title);
    }

    #endregion

    #region Multiple Updates Tests

    [Fact]
    public async Task Handle_WhenUpdatingMultipleTimes_EachUpdateIsApplied()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mapper = CreateMapper();
        var logger = CreateLogger();
        var eventService = CreateEventService();
        var messageProducer = CreateMessageProducer();
        var handler = new Edit.Handler(context, mapper, logger, eventService, messageProducer);
        
        var existingTaskItem = CreateValidTaskItem();
        await context.TaskItems.AddAsync(existingTaskItem);
        await context.SaveChangesAsync();

        // First update
        var firstUpdate = CreateValidTaskItem();
        firstUpdate.Id = existingTaskItem.Id;
        firstUpdate.Title = "First Update";
        await handler.Handle(new Edit.Command { TaskItem = firstUpdate }, CancellationToken.None);

        // Second update
        var secondUpdate = CreateValidTaskItem();
        secondUpdate.Id = existingTaskItem.Id;
        secondUpdate.Title = "Second Update";
        await handler.Handle(new Edit.Command { TaskItem = secondUpdate }, CancellationToken.None);

        // Assert
        var savedItem = await context.TaskItems.FindAsync(existingTaskItem.Id);
        Assert.Equal("Second Update", savedItem.Title);
    }

    #endregion
}

