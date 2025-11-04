/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using Microsoft.Extensions.Logging;
using TaskForge.Domain;
using TaskForge.Persistence;
using Tests.TaskForge.Core.Helpers;

namespace Tests.TaskForge.Core.Fixtures;

/// <summary>
/// Base fixture for handler tests that provides common setup and helper methods
/// </summary>
public abstract class BaseHandlerTestFixture : IDisposable
{
    protected DataContext Context { get; private set; }
    
    protected BaseHandlerTestFixture()
    {
        Context = DatabaseTestHelper.CreateInMemoryContext();
    }

    /// <summary>
    /// Creates a logger for the specified handler type
    /// </summary>
    protected ILogger<T> CreateLogger<T>() => LoggerTestHelper.CreateLogger<T>();

    /// <summary>
    /// Creates a valid TaskItem for testing
    /// </summary>
    protected TaskItem CreateValidTaskItem(Action<TaskItem>? customize = null) => 
        TestDataFactory.CreateValidTaskItem(customize);

    /// <summary>
    /// Creates a valid TaskItem with a specific ID
    /// </summary>
    protected TaskItem CreateTaskItemWithId(Guid id, Action<TaskItem>? customize = null) => 
        TestDataFactory.CreateTaskItemWithId(id, customize);

    /// <summary>
    /// Creates a cancelled CancellationToken for testing cancellation scenarios
    /// </summary>
    protected CancellationToken CreateCancelledToken() => 
        CancellationTokenTestHelper.CreateCancelledToken();

    /// <summary>
    /// Adds a task item to the context and saves it
    /// </summary>
    protected async Task<TaskItem> AddTaskItemToContextAsync(TaskItem taskItem)
    {
        await Context.TaskItems.AddAsync(taskItem);
        await Context.SaveChangesAsync();
        return taskItem;
    }

    /// <summary>
    /// Adds multiple task items to the context and saves them
    /// </summary>
    protected async Task<List<TaskItem>> AddTaskItemsToContextAsync(params TaskItem[] taskItems)
    {
        await Context.TaskItems.AddRangeAsync(taskItems);
        await Context.SaveChangesAsync();
        return taskItems.ToList();
    }

    /// <summary>
    /// Clears all task items from the context
    /// </summary>
    protected async Task ClearTaskItemsAsync()
    {
        Context.TaskItems.RemoveRange(Context.TaskItems);
        await Context.SaveChangesAsync();
    }

    public virtual void Dispose()
    {
        Context?.Dispose();
    }
}

