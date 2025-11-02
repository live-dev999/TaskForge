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
using TaskForge.Domain;

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
        var messageId = context.MessageId?.ToString() ?? "unknown";
        var correlationId = context.CorrelationId?.ToString() ?? "none";
        var conversationId = context.ConversationId?.ToString() ?? "none";
        
        _logger.LogInformation(
            "[RECEIVE] Starting to consume task change event: MessageId={MessageId}, CorrelationId={CorrelationId}, ConversationId={ConversationId}, SourceAddress={SourceAddress}",
            messageId,
            correlationId,
            conversationId,
            context.SourceAddress?.ToString() ?? "unknown");

        var eventDto = context.Message;

        if (eventDto == null)
        {
            _logger.LogError("[ERROR] Received null task change event: MessageId={MessageId}", messageId);
            return;
        }

        if (eventDto.TaskId == Guid.Empty)
        {
            _logger.LogWarning("[WARN] Received task change event with empty TaskId: MessageId={MessageId}", messageId);
        }

        try
        {
            _logger.LogInformation(
                "Processing task change event: MessageId={MessageId}, TaskId={TaskId}, EventType={EventType}, Title={Title}, " +
                "Status={Status}, EventTimestamp={EventTimestamp}, CreatedAt={CreatedAt}, UpdatedAt={UpdatedAt}",
                messageId,
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
                _logger.LogDebug("Task description: MessageId={MessageId}, Description={Description}", messageId, eventDto.Description);
            }

            // Simulate processing time (for demonstration)
            await Task.Delay(10, context.CancellationToken);

            _logger.LogInformation(
                "[OK] Successfully processed task change event: MessageId={MessageId}, TaskId={TaskId}, EventType={EventType}",
                messageId,
                eventDto.TaskId,
                eventDto.EventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "[ERROR] Error processing task change event: MessageId={MessageId}, TaskId={TaskId}, EventType={EventType}, Error={ErrorMessage}",
                messageId,
                eventDto?.TaskId ?? Guid.Empty,
                eventDto?.EventType ?? "unknown",
                ex.Message);
            throw; // Re-throw to let MassTransit handle retry
        }
    }
}

