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

using MassTransit;
using Microsoft.Extensions.Logging;

namespace TaskForge.Application.Core;

/// <summary>
/// Service for publishing task change events to RabbitMQ message queue using MassTransit.
/// </summary>
public class MessageProducer : IMessageProducer
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MessageProducer> _logger;

    public MessageProducer(IPublishEndpoint publishEndpoint, ILogger<MessageProducer> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    /// <summary>
    /// Publishes a task change event to the RabbitMQ message queue.
    /// </summary>
    /// <param name="eventDto">The task change event data to publish.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when eventDto is null.</exception>
    /// <exception cref="ArgumentException">Thrown when eventDto has invalid data (e.g., empty TaskId).</exception>
    public async Task PublishEventAsync(TaskChangeEventDto eventDto, CancellationToken cancellationToken = default)
    {
        if (eventDto == null)
        {
            _logger.LogError("Cannot publish null event");
            throw new ArgumentNullException(nameof(eventDto));
        }

        if (eventDto.TaskId == Guid.Empty)
        {
            _logger.LogError("Cannot publish event with empty TaskId");
            throw new ArgumentException("TaskId cannot be empty", nameof(eventDto));
        }

        try
        {
            _logger.LogInformation(
                "Publishing task change event to RabbitMQ: TaskId={TaskId}, EventType={EventType}",
                eventDto.TaskId,
                eventDto.EventType);

            // MassTransit will automatically serialize TaskChangeEventDto and publish it
            // The message will be routed to "task-change-events" exchange/queue based on configuration
            await _publishEndpoint.Publish(eventDto, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation(
                "Successfully published task change event to RabbitMQ: TaskId={TaskId}, EventType={EventType}",
                eventDto.TaskId,
                eventDto.EventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error publishing task change event to RabbitMQ: TaskId={TaskId}, EventType={EventType}, Error={ErrorMessage}",
                eventDto.TaskId,
                eventDto.EventType,
                ex.Message);
            // Don't throw - allow the operation to continue even if RabbitMQ is unavailable
            // This ensures the main operation (create/edit/delete) succeeds even if messaging fails
        }
    }
}

