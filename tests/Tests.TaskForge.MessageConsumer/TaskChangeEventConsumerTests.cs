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
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using TaskForge.Application.Core;
using TaskForge.MessageConsumer.Consumers;
using Xunit;

namespace Tests.TaskForge.MessageConsumer;

public class TaskChangeEventConsumerTests
{
    private readonly Mock<ILogger<TaskChangeEventConsumer>> _loggerMock;
    private readonly TaskChangeEventConsumer _consumer;

    public TaskChangeEventConsumerTests()
    {
        _loggerMock = new Mock<ILogger<TaskChangeEventConsumer>>();
        _consumer = new TaskChangeEventConsumer(_loggerMock.Object);
    }

    [Fact]
    public async Task Consume_WithValidCreatedEvent_ShouldLogInformation()
    {
        // Arrange
        var eventDto = new TaskChangeEventDto
        {
            TaskId = Guid.NewGuid(),
            EventType = "Created",
            Title = "Test Task",
            Description = "Test Description",
            Status = "New",
            EventTimestamp = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var contextMock = new Mock<ConsumeContext<TaskChangeEventDto>>();
        contextMock.Setup(x => x.Message).Returns(eventDto);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        // Consumer logs multiple Information messages: [RECEIVE], Processing, [OK]
        // So we check that at least one Information log was called
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => ContainsLogMessage(v, "Processing") || ContainsLogMessage(v, "[RECEIVE]") || ContainsLogMessage(v, "[OK]")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!),
            Times.AtLeastOnce);

        // Verify Debug log for description
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => ContainsLogMessage(v, "Task description")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!),
            Times.Once);
    }

    [Fact]
    public async Task Consume_WithValidUpdatedEvent_ShouldLogInformation()
    {
        // Arrange
        var eventDto = new TaskChangeEventDto
        {
            TaskId = Guid.NewGuid(),
            EventType = "Updated",
            Title = "Updated Task",
            Description = "Updated Description",
            Status = "InProgress",
            EventTimestamp = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow
        };

        var contextMock = new Mock<ConsumeContext<TaskChangeEventDto>>();
        contextMock.Setup(x => x.Message).Returns(eventDto);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        // Consumer logs multiple Information messages: [RECEIVE], Processing, [OK]
        // So we check that at least one Information log was called
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => ContainsLogMessage(v, "Processing") || ContainsLogMessage(v, "[RECEIVE]") || ContainsLogMessage(v, "[OK]")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task Consume_WithValidDeletedEvent_ShouldLogInformation()
    {
        // Arrange
        var eventDto = new TaskChangeEventDto
        {
            TaskId = Guid.NewGuid(),
            EventType = "Deleted",
            Title = "Deleted Task",
            Description = null,
            Status = null,
            EventTimestamp = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow
        };

        var contextMock = new Mock<ConsumeContext<TaskChangeEventDto>>();
        contextMock.Setup(x => x.Message).Returns(eventDto);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        // Consumer logs multiple Information messages: [RECEIVE], Processing, [OK]
        // So we check that at least one Information log was called
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => ContainsLogMessage(v, "Processing") || ContainsLogMessage(v, "[RECEIVE]") || ContainsLogMessage(v, "[OK]")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!),
            Times.AtLeastOnce);

        // Verify Debug log for description should NOT be called when description is null
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => ContainsLogMessage(v, "Task description")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!),
            Times.Never);
    }

    [Fact]
    public async Task Consume_WithNullMessage_ShouldLogError()
    {
        // Arrange
        TaskChangeEventDto? eventDto = null;

        var contextMock = new Mock<ConsumeContext<TaskChangeEventDto>>();
        contextMock.Setup(x => x.Message).Returns(eventDto!);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => ContainsLogMessage(v, "Received null task change event")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!),
            Times.Once);

        // [RECEIVE] log is called before null check, but Processing should NOT be called
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => ContainsLogMessage(v, "Processing task change event")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!),
            Times.Never);
    }

    [Fact]
    public async Task Consume_WithEmptyTaskId_ShouldLogWarning()
    {
        // Arrange
        var eventDto = new TaskChangeEventDto
        {
            TaskId = Guid.Empty,
            EventType = "Created",
            Title = "Test Task",
            EventTimestamp = DateTime.UtcNow
        };

        var contextMock = new Mock<ConsumeContext<TaskChangeEventDto>>();
        contextMock.Setup(x => x.Message).Returns(eventDto);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => ContainsLogMessage(v, "Received task change event with empty TaskId")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!),
            Times.Once);

        // Should still log Information
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => ContainsLogMessage(v, "Processing task change event")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!),
            Times.Once);
    }

    [Fact]
    public async Task Consume_WithEmptyDescription_ShouldNotLogDebug()
    {
        // Arrange
        var eventDto = new TaskChangeEventDto
        {
            TaskId = Guid.NewGuid(),
            EventType = "Created",
            Title = "Test Task",
            Description = string.Empty,
            EventTimestamp = DateTime.UtcNow
        };

        var contextMock = new Mock<ConsumeContext<TaskChangeEventDto>>();
        contextMock.Setup(x => x.Message).Returns(eventDto);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        // Verify Debug log for description should NOT be called when description is empty
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => ContainsLogMessage(v, "Task description")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!),
            Times.Never);

        // But Information log should be called (at least [RECEIVE] and Processing)
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => ContainsLogMessage(v, "Processing") || ContainsLogMessage(v, "[RECEIVE]") || ContainsLogMessage(v, "[OK]")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task Consume_WithAllEventTypes_ShouldLogAll()
    {
        // Arrange
        var createdEvent = new TaskChangeEventDto
        {
            TaskId = Guid.NewGuid(),
            EventType = "Created",
            EventTimestamp = DateTime.UtcNow
        };

        var updatedEvent = new TaskChangeEventDto
        {
            TaskId = Guid.NewGuid(),
            EventType = "Updated",
            EventTimestamp = DateTime.UtcNow
        };

        var deletedEvent = new TaskChangeEventDto
        {
            TaskId = Guid.NewGuid(),
            EventType = "Deleted",
            EventTimestamp = DateTime.UtcNow
        };

        var createdContextMock = new Mock<ConsumeContext<TaskChangeEventDto>>();
        createdContextMock.Setup(x => x.Message).Returns(createdEvent);

        var updatedContextMock = new Mock<ConsumeContext<TaskChangeEventDto>>();
        updatedContextMock.Setup(x => x.Message).Returns(updatedEvent);

        var deletedContextMock = new Mock<ConsumeContext<TaskChangeEventDto>>();
        deletedContextMock.Setup(x => x.Message).Returns(deletedEvent);

        // Act
        await _consumer.Consume(createdContextMock.Object);
        await _consumer.Consume(updatedContextMock.Object);
        await _consumer.Consume(deletedContextMock.Object);

        // Assert
        // Each event triggers multiple Information logs: [RECEIVE], Processing, [OK]
        // So we verify at least 3 calls (one per event, though there will be more)
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => ContainsLogMessage(v, "Processing") || ContainsLogMessage(v, "[RECEIVE]") || ContainsLogMessage(v, "[OK]")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!),
            Times.AtLeast(3));
    }

    [Fact]
    public async Task Consume_WithAllEventProperties_ShouldLogAllProperties()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var createdAt = now.AddDays(-1);
        var updatedAt = now;

        var eventDto = new TaskChangeEventDto
        {
            TaskId = taskId,
            EventType = "Updated",
            Title = "Complete Task Title",
            Description = "Complete Task Description",
            Status = "Completed",
            EventTimestamp = now,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        var contextMock = new Mock<ConsumeContext<TaskChangeEventDto>>();
        contextMock.Setup(x => x.Message).Returns(eventDto);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        // Verify that all properties are logged in the Information message
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => 
                    ContainsLogProperty(v, "TaskId", taskId.ToString()) &&
                    ContainsLogProperty(v, "EventType", "Updated") &&
                    ContainsLogProperty(v, "Title", "Complete Task Title") &&
                    ContainsLogProperty(v, "Status", "Completed")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)!),
            Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldCompleteSuccessfully_WithoutThrowing()
    {
        // Arrange
        var eventDto = new TaskChangeEventDto
        {
            TaskId = Guid.NewGuid(),
            EventType = "Created",
            EventTimestamp = DateTime.UtcNow
        };

        var contextMock = new Mock<ConsumeContext<TaskChangeEventDto>>();
        contextMock.Setup(x => x.Message).Returns(eventDto);

        // Act
        Func<Task> act = async () => await _consumer.Consume(contextMock.Object);

        // Assert
        await act.Should().NotThrowAsync();
    }

    // Helper methods for verifying log messages
    private static bool ContainsLogMessage(object logValue, string expectedMessage)
    {
        if (logValue is IReadOnlyList<KeyValuePair<string, object>> state)
        {
            var message = state.FirstOrDefault(kvp => kvp.Key == "{OriginalFormat}").Value?.ToString() 
                       ?? state.FirstOrDefault(kvp => kvp.Key == "Message").Value?.ToString();
            return message != null && message.Contains(expectedMessage, StringComparison.OrdinalIgnoreCase);
        }
        return logValue.ToString()?.Contains(expectedMessage, StringComparison.OrdinalIgnoreCase) ?? false;
    }

    private static bool ContainsLogProperty(object logValue, string propertyName, string propertyValue)
    {
        if (logValue is IReadOnlyList<KeyValuePair<string, object>> state)
        {
            var value = state.FirstOrDefault(kvp => kvp.Key == propertyName).Value?.ToString();
            return value != null && value.Contains(propertyValue, StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }
}

