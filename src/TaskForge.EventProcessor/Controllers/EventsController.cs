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

using Microsoft.AspNetCore.Mvc;
using TaskForge.EventProcessor.Models;
using TaskForge.EventProcessor.Services;

namespace TaskForge.EventProcessor.Controllers;

/// <summary>
/// Controller for handling task change events.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventLogger _eventLogger;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IEventLogger eventLogger, ILogger<EventsController> logger)
    {
        _eventLogger = eventLogger;
        _logger = logger;
    }

    /// <summary>
    /// Receives a task change event from the API service.
    /// </summary>
    /// <param name="taskEvent">The task change event.</param>
    /// <returns>OK if the event was logged successfully.</returns>
    [HttpPost]
    public IActionResult PostEvent([FromBody] TaskChangeEvent taskEvent)
    {
        if (taskEvent == null)
        {
            _logger.LogWarning("Received null task event");
            return BadRequest("Task event is required");
        }

        if (taskEvent.TaskId == Guid.Empty)
        {
            _logger.LogWarning("Received task event with empty TaskId");
            return BadRequest("TaskId is required");
        }

        _eventLogger.LogEvent(taskEvent);
        return Ok(new { message = "Event logged successfully", taskId = taskEvent.TaskId });
    }

    /// <summary>
    /// Gets all task change events.
    /// </summary>
    /// <returns>A collection of all logged events.</returns>
    [HttpGet]
    public IActionResult GetAllEvents()
    {
        var events = _eventLogger.GetAllEvents();
        return Ok(events);
    }

    /// <summary>
    /// Gets task change events for a specific task.
    /// </summary>
    /// <param name="taskId">The task ID to filter by.</param>
    /// <returns>A collection of events for the specified task.</returns>
    [HttpGet("{taskId}")]
    public IActionResult GetEventsByTaskId(Guid taskId)
    {
        if (taskId == Guid.Empty)
        {
            return BadRequest("Invalid task ID");
        }

        var events = _eventLogger.GetEventsByTaskId(taskId);
        return Ok(events);
    }
}

