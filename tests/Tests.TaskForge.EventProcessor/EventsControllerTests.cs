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

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TaskForge.EventProcessor.Controllers;
using TaskForge.EventProcessor.Models;
using TaskForge.EventProcessor.Services;

namespace Tests.TaskForge.EventProcessor;

public class EventsControllerTests
{
    private readonly Mock<IEventLogger> _mockEventLogger;
    private readonly Mock<ILogger<EventsController>> _mockLogger;
    private readonly EventsController _controller;

    public EventsControllerTests()
    {
        _mockEventLogger = new Mock<IEventLogger>();
        _mockLogger = new Mock<ILogger<EventsController>>();
        _controller = new EventsController(_mockEventLogger.Object, _mockLogger.Object);
    }

    [Fact]
    public void PostEvent_WithValidEvent_ShouldReturnOkAndLogEvent()
    {
        // Arrange
        var taskEvent = new TaskChangeEvent
        {
            TaskId = Guid.NewGuid(),
            EventType = "Created",
            Title = "Test Task",
            Description = "Test Description",
            Status = "New",
            EventTimestamp = DateTime.UtcNow
        };

        // Act
        var result = _controller.PostEvent(taskEvent);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockEventLogger.Verify(x => x.LogEvent(taskEvent), Times.Once);
    }

    [Fact]
    public void PostEvent_WithNullEvent_ShouldReturnBadRequest()
    {
        // Arrange
        TaskChangeEvent? taskEvent = null;

        // Act
        var result = _controller.PostEvent(taskEvent);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _mockEventLogger.Verify(x => x.LogEvent(It.IsAny<TaskChangeEvent>()), Times.Never);
    }

    [Fact]
    public void PostEvent_WithEmptyTaskId_ShouldReturnBadRequest()
    {
        // Arrange
        var taskEvent = new TaskChangeEvent
        {
            TaskId = Guid.Empty,
            EventType = "Created"
        };

        // Act
        var result = _controller.PostEvent(taskEvent);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _mockEventLogger.Verify(x => x.LogEvent(It.IsAny<TaskChangeEvent>()), Times.Never);
    }

    [Fact]
    public void GetAllEvents_ShouldReturnAllEvents()
    {
        // Arrange
        var events = new List<TaskChangeEvent>
        {
            new TaskChangeEvent { TaskId = Guid.NewGuid(), EventType = "Created" },
            new TaskChangeEvent { TaskId = Guid.NewGuid(), EventType = "Updated" }
        };

        _mockEventLogger.Setup(x => x.GetAllEvents()).Returns(events);

        // Act
        var result = _controller.GetAllEvents();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(events);
        _mockEventLogger.Verify(x => x.GetAllEvents(), Times.Once);
    }

    [Fact]
    public void GetEventsByTaskId_WithValidTaskId_ShouldReturnEventsForTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var events = new List<TaskChangeEvent>
        {
            new TaskChangeEvent { TaskId = taskId, EventType = "Created" },
            new TaskChangeEvent { TaskId = taskId, EventType = "Updated" }
        };

        _mockEventLogger.Setup(x => x.GetEventsByTaskId(taskId)).Returns(events);

        // Act
        var result = _controller.GetEventsByTaskId(taskId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(events);
        _mockEventLogger.Verify(x => x.GetEventsByTaskId(taskId), Times.Once);
    }

    [Fact]
    public void GetEventsByTaskId_WithEmptyTaskId_ShouldReturnBadRequest()
    {
        // Arrange
        var taskId = Guid.Empty;

        // Act
        var result = _controller.GetEventsByTaskId(taskId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _mockEventLogger.Verify(x => x.GetEventsByTaskId(It.IsAny<Guid>()), Times.Never);
    }
}

