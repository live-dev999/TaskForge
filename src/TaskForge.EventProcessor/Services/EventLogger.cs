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

using Microsoft.Extensions.Logging;
using TaskForge.EventProcessor.Models;

namespace TaskForge.EventProcessor.Services;

/// <summary>
/// Service for logging and storing task change events.
/// </summary>
public class EventLogger : IEventLogger
{
    private readonly ILogger<EventLogger> _logger;
    private readonly List<TaskChangeEvent> _events = new();
    private readonly object _lock = new();

    public EventLogger(ILogger<EventLogger> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Logs a task change event and stores it in memory.
    /// </summary>
    /// <param name="taskEvent">The task change event to log.</param>
    public void LogEvent(TaskChangeEvent taskEvent)
    {
        if (taskEvent == null)
        {
            _logger.LogWarning("Attempted to log null task event");
            return;
        }

        lock (_lock)
        {
            _events.Add(taskEvent);
        }

        _logger.LogInformation(
            "Task event received: TaskId={TaskId}, EventType={EventType}, Title={Title}, Status={Status}, EventTimestamp={EventTimestamp}",
            taskEvent.TaskId,
            taskEvent.EventType,
            taskEvent.Title,
            taskEvent.Status,
            taskEvent.EventTimestamp);
    }

    /// <summary>
    /// Gets all logged events.
    /// </summary>
    /// <returns>A collection of all logged events.</returns>
    public IEnumerable<TaskChangeEvent> GetAllEvents()
    {
        lock (_lock)
        {
            return _events.ToList();
        }
    }

    /// <summary>
    /// Gets events by task ID.
    /// </summary>
    /// <param name="taskId">The task ID to filter by.</param>
    /// <returns>A collection of events for the specified task.</returns>
    public IEnumerable<TaskChangeEvent> GetEventsByTaskId(Guid taskId)
    {
        lock (_lock)
        {
            return _events
                .Where(e => e.TaskId == taskId)
                .ToList();
        }
    }
}

