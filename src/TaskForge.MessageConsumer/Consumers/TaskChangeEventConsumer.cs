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
using TaskForge.Application.Core;

namespace TaskForge.MessageConsumer.Consumers;

/// <summary>
/// MassTransit consumer for processing task change events from RabbitMQ.
/// </summary>
public class TaskChangeEventConsumer : IConsumer<TaskChangeEventDto>
{
    private readonly ILogger<TaskChangeEventConsumer> _logger;

    public TaskChangeEventConsumer(ILogger<TaskChangeEventConsumer> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Consumes a task change event message and logs it.
    /// </summary>
    /// <param name="context">The consume context containing the message.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Consume(ConsumeContext<TaskChangeEventDto> context)
    {
        var eventDto = context.Message;

        if (eventDto == null)
        {
            _logger.LogError("Received null task change event");
            return;
        }

        if (eventDto.TaskId == Guid.Empty)
        {
            _logger.LogWarning("Received task change event with empty TaskId");
        }

        _logger.LogInformation(
            "Received task change event: TaskId={TaskId}, EventType={EventType}, Title={Title}, " +
            "Status={Status}, EventTimestamp={EventTimestamp}, CreatedAt={CreatedAt}, UpdatedAt={UpdatedAt}",
            eventDto.TaskId,
            eventDto.EventType,
            eventDto.Title,
            eventDto.Status,
            eventDto.EventTimestamp,
            eventDto.CreatedAt,
            eventDto.UpdatedAt);

        // Log description separately if it exists (to avoid cluttering main log)
        if (!string.IsNullOrEmpty(eventDto.Description))
        {
            _logger.LogDebug("Task description: {Description}", eventDto.Description);
        }

        await Task.CompletedTask;
    }
}

