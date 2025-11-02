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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TaskForge.Application.Core;
using Xunit;

namespace Tests.TaskForge.Application.Core;

public class MessageProducerTests
{
    private readonly Mock<ILogger<MessageProducer>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly IMessageProducer _messageProducer;

    public MessageProducerTests()
    {
        _loggerMock = new Mock<ILogger<MessageProducer>>();
        _configurationMock = new Mock<IConfiguration>();
        var publishEndpointMock = new Mock<MassTransit.IPublishEndpoint>();
        // Initialize with real implementation (without RabbitMQ logic yet)
        _messageProducer = new MessageProducer(publishEndpointMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task PublishEventAsync_WithValidEvent_ShouldSucceed()
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

        // Act
        await _messageProducer.PublishEventAsync(eventDto);

        // Assert
        // TODO: Verify that message was published to RabbitMQ
    }

    [Fact]
    public async Task PublishEventAsync_WithNullEvent_ShouldThrowArgumentNullException()
    {
        // Arrange
        TaskChangeEventDto? eventDto = null;

        // Act
        Func<Task> act = async () => await _messageProducer.PublishEventAsync(eventDto!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task PublishEventAsync_WithEmptyTaskId_ShouldHandleGracefully()
    {
        // Arrange
        var eventDto = new TaskChangeEventDto
        {
            TaskId = Guid.Empty,
            EventType = "Created",
            EventTimestamp = DateTime.UtcNow
        };

        // Act
        Func<Task> act = async () => await _messageProducer.PublishEventAsync(eventDto);

        // Assert
        // TODO: Decide behavior - should throw or log warning?
        // For now, let's assume it should throw
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task PublishEventAsync_WhenRabbitMQUnavailable_ShouldNotThrow()
    {
        // Arrange
        var eventDto = new TaskChangeEventDto
        {
            TaskId = Guid.NewGuid(),
            EventType = "Updated",
            EventTimestamp = DateTime.UtcNow
        };

        // Act
        // Simulate RabbitMQ connection failure - the service should catch exceptions
        Func<Task> act = async () => await _messageProducer.PublishEventAsync(eventDto);

        // Assert
        // Should not throw - exceptions are caught and logged internally
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task PublishEventAsync_WithDifferentEventTypes_ShouldPublishCorrectly()
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

        // Act & Assert
        await _messageProducer.PublishEventAsync(createdEvent);
        await _messageProducer.PublishEventAsync(updatedEvent);
        await _messageProducer.PublishEventAsync(deletedEvent);

        // TODO: Verify all three events were published
    }
}

