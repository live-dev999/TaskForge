/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 *
 *   Permission is hereby granted, free of charge, to any person obtaining a copy
 *   of this software and associated documentation files (the "Software"), to deal
 *   in the Software without restriction, including without limitation the rights
 *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *   copies of the Software, and to permit persons to whom the Software is
 *   furnished to do so, subject to the following conditions:
 * 
 *   The above copyright notice and this permission notice shall be included in all
 *   copies or substantial portions of the Software.
 * 
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 */

using TaskForge.Domain;

namespace TaskForge.Application.Core;

/// <summary>
/// Extension methods for TaskItem entity.
/// </summary>
public static class TaskItemExtensions
{
    /// <summary>
    /// Maps a TaskItem to a TaskChangeEventDto.
    /// </summary>
    /// <param name="taskItem">The task item to map.</param>
    /// <param name="eventType">The type of event (e.g., "Created", "Updated", "Deleted").</param>
    /// <returns>A TaskChangeEventDto representing the task change event.</returns>
    public static TaskChangeEventDto ToEventDto(this TaskItem taskItem, string eventType)
    {
        return new TaskChangeEventDto
        {
            TaskId = taskItem.Id,
            EventType = eventType,
            Title = taskItem.Title,
            Description = taskItem.Description,
            Status = taskItem.Status.ToString(),
            EventTimestamp = DateTime.UtcNow,
            CreatedAt = taskItem.CreatedAt,
            UpdatedAt = taskItem.UpdatedAt
        };
    }
}

