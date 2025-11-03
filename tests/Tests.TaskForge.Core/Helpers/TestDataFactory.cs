/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using TaskForge.Domain;
using TaskForge.Domain.Enum;

namespace Tests.TaskForge.Core.Helpers;

/// <summary>
/// Factory for creating test data objects
/// </summary>
public static class TestDataFactory
{
    /// <summary>
    /// Creates a valid TaskItem with default test values
    /// </summary>
    /// <param name="customize">Optional action to customize the task item</param>
    /// <returns>TaskItem instance with valid test data</returns>
    public static TaskItem CreateValidTaskItem(Action<TaskItem>? customize = null)
    {
        var taskItem = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Title",
            Description = "Test Description",
            Status = TaskItemStatus.New,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        customize?.Invoke(taskItem);
        return taskItem;
    }

    /// <summary>
    /// Creates a TaskItem with a specific ID
    /// </summary>
    public static TaskItem CreateTaskItemWithId(Guid id, Action<TaskItem>? customize = null)
    {
        return CreateValidTaskItem(item =>
        {
            item.Id = id;
            customize?.Invoke(item);
        });
    }

    /// <summary>
    /// Creates a TaskItem with a specific title
    /// </summary>
    public static TaskItem CreateTaskItemWithTitle(string title, Action<TaskItem>? customize = null)
    {
        return CreateValidTaskItem(item =>
        {
            item.Title = title;
            customize?.Invoke(item);
        });
    }

    /// <summary>
    /// Creates a TaskItem with a specific status
    /// </summary>
    public static TaskItem CreateTaskItemWithStatus(TaskItemStatus status, Action<TaskItem>? customize = null)
    {
        return CreateValidTaskItem(item =>
        {
            item.Status = status;
            customize?.Invoke(item);
        });
    }

    /// <summary>
    /// Creates multiple TaskItems
    /// </summary>
    /// <param name="count">Number of items to create</param>
    /// <param name="customize">Optional action to customize each item</param>
    /// <returns>List of TaskItems</returns>
    public static List<TaskItem> CreateTaskItems(int count, Action<TaskItem, int>? customize = null)
    {
        var items = new List<TaskItem>();
        for (int i = 0; i < count; i++)
        {
            var item = CreateValidTaskItem();
            customize?.Invoke(item, i);
            items.Add(item);
        }
        return items;
    }

    /// <summary>
    /// Creates a TaskItem with empty GUID (for testing edge cases)
    /// </summary>
    public static TaskItem CreateTaskItemWithEmptyId(Action<TaskItem>? customize = null)
    {
        return CreateValidTaskItem(item =>
        {
            item.Id = Guid.Empty;
            customize?.Invoke(item);
        });
    }

    /// <summary>
    /// Creates a TaskItem with null title (for validation testing)
    /// </summary>
    public static TaskItem CreateTaskItemWithNullTitle(Action<TaskItem>? customize = null)
    {
        return CreateValidTaskItem(item =>
        {
            item.Title = null;
            customize?.Invoke(item);
        });
    }

    /// <summary>
    /// Creates a TaskItem with a very long title (for edge case testing)
    /// </summary>
    public static TaskItem CreateTaskItemWithLongTitle(int length = 10000, Action<TaskItem>? customize = null)
    {
        return CreateValidTaskItem(item =>
        {
            item.Title = new string('A', length);
            customize?.Invoke(item);
        });
    }
}

