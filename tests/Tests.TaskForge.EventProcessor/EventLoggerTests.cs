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
using Microsoft.Extensions.Logging;
using Moq;
using TaskForge.EventProcessor.Models;
using TaskForge.EventProcessor.Services;

namespace Tests.TaskForge.EventProcessor;

public class EventLoggerTests
{
    private readonly Mock<ILogger<EventLogger>> _mockLogger;
    private readonly EventLogger _eventLogger;

    public EventLoggerTests()
    {
        _mockLogger = new Mock<ILogger<EventLogger>>();
        _eventLogger = new EventLogger(_mockLogger.Object);
    }

    [Fact]
    public void LogEvent_WithValidEvent_ShouldStoreEvent()
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
        _eventLogger.LogEvent(taskEvent);

        // Assert
        var events = _eventLogger.GetAllEvents();
        events.Should().ContainSingle(e => e.TaskId == taskEvent.TaskId && e.EventType == taskEvent.EventType);
    }

    [Fact]
    public void LogEvent_WithNullEvent_ShouldNotThrow()
    {
        // Act
        Action act = () => _eventLogger.LogEvent(null!);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void GetAllEvents_ShouldReturnAllLoggedEvents()
    {
        // Arrange
        var event1 = new TaskChangeEvent { TaskId = Guid.NewGuid(), EventType = "Created" };
        var event2 = new TaskChangeEvent { TaskId = Guid.NewGuid(), EventType = "Updated" };
        var event3 = new TaskChangeEvent { TaskId = Guid.NewGuid(), EventType = "Deleted" };

        // Act
        _eventLogger.LogEvent(event1);
        _eventLogger.LogEvent(event2);
        _eventLogger.LogEvent(event3);

        // Assert
        var events = _eventLogger.GetAllEvents();
        events.Should().HaveCount(3);
        events.Should().Contain(e => e.TaskId == event1.TaskId);
        events.Should().Contain(e => e.TaskId == event2.TaskId);
        events.Should().Contain(e => e.TaskId == event3.TaskId);
    }

    [Fact]
    public void GetEventsByTaskId_ShouldReturnOnlyEventsForSpecificTask()
    {
        // Arrange
        var taskId1 = Guid.NewGuid();
        var taskId2 = Guid.NewGuid();

        var event1 = new TaskChangeEvent { TaskId = taskId1, EventType = "Created" };
        var event2 = new TaskChangeEvent { TaskId = taskId1, EventType = "Updated" };
        var event3 = new TaskChangeEvent { TaskId = taskId2, EventType = "Created" };

        _eventLogger.LogEvent(event1);
        _eventLogger.LogEvent(event2);
        _eventLogger.LogEvent(event3);

        // Act
        var eventsForTask1 = _eventLogger.GetEventsByTaskId(taskId1);

        // Assert
        eventsForTask1.Should().HaveCount(2);
        eventsForTask1.Should().OnlyContain(e => e.TaskId == taskId1);
    }

    [Fact]
    public void GetEventsByTaskId_WithNonExistentTaskId_ShouldReturnEmptyCollection()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        // Act
        var events = _eventLogger.GetEventsByTaskId(taskId);

        // Assert
        events.Should().BeEmpty();
    }

    [Fact]
    public void LogEvent_ShouldBeThreadSafe()
    {
        // Arrange
        var events = new List<TaskChangeEvent>();
        for (int i = 0; i < 100; i++)
        {
            events.Add(new TaskChangeEvent
            {
                TaskId = Guid.NewGuid(),
                EventType = "Created",
                EventTimestamp = DateTime.UtcNow
            });
        }

        // Act
        Parallel.ForEach(events, e => _eventLogger.LogEvent(e));

        // Assert
        var allEvents = _eventLogger.GetAllEvents();
        allEvents.Should().HaveCount(100);
    }
}

