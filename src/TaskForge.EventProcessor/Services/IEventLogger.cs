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

using TaskForge.EventProcessor.Models;

namespace TaskForge.EventProcessor.Services;

/// <summary>
/// Interface for logging task change events.
/// </summary>
public interface IEventLogger
{
    /// <summary>
    /// Logs a task change event.
    /// </summary>
    /// <param name="taskEvent">The task change event to log.</param>
    void LogEvent(TaskChangeEvent taskEvent);

    /// <summary>
    /// Gets all logged events.
    /// </summary>
    /// <returns>A collection of all logged events.</returns>
    IEnumerable<TaskChangeEvent> GetAllEvents();

    /// <summary>
    /// Gets events by task ID.
    /// </summary>
    /// <param name="taskId">The task ID to filter by.</param>
    /// <returns>A collection of events for the specified task.</returns>
    IEnumerable<TaskChangeEvent> GetEventsByTaskId(Guid taskId);
}

