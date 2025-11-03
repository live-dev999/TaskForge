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
using TaskForge.MessageConsumer.Models;

namespace TaskForge.MessageConsumer.Services;

/// <summary>
/// Service for consuming task change events from RabbitMQ message queue and logging them.
/// Note: With MassTransit, the actual consumption is handled by TaskChangeEventConsumer.
/// This service is kept for backward compatibility and can be used for additional processing.
/// </summary>
public class MessageConsumerService : IMessageConsumerService
{
    private readonly ILogger<MessageConsumerService> _logger;

    public MessageConsumerService(ILogger<MessageConsumerService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Starts consuming messages from the RabbitMQ queue.
    /// Note: With MassTransit, consumption is handled automatically via hosted service.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to stop consuming.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MessageConsumerService.StartConsumingAsync called - MassTransit handles consumption automatically");
        await Task.CompletedTask;
    }

    /// <summary>
    /// Processes a received task change event message.
    /// Note: With MassTransit, this is called by TaskChangeEventConsumer.Consume.
    /// </summary>
    /// <param name="eventData">The task change event data to process.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when eventData is null.</exception>
    public async Task ProcessMessageAsync(TaskChangeEvent eventData)
    {
        if (eventData == null)
        {
            _logger.LogError("Cannot process null event");
            throw new ArgumentNullException(nameof(eventData));
        }

        _logger.LogInformation(
            "Processing task change event: TaskId={TaskId}, EventType={EventType}",
            eventData.TaskId,
            eventData.EventType);

        await Task.CompletedTask;
    }

    /// <summary>
    /// Stops consuming messages from the RabbitMQ queue.
    /// Note: With MassTransit, shutdown is handled automatically.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task StopConsumingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MessageConsumerService.StopConsumingAsync called - MassTransit handles shutdown automatically");
        await Task.CompletedTask;
    }
}

