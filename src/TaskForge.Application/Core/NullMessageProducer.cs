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
using TaskForge.Domain;

namespace TaskForge.Application.Core;

/// <summary>
/// Null implementation of IMessageProducer that does nothing.
/// Used when RabbitMQ is not available or disabled (e.g., local development without Docker).
/// </summary>
public class NullMessageProducer : IMessageProducer
{
    private readonly ILogger<NullMessageProducer> _logger;

    public NullMessageProducer(ILogger<NullMessageProducer> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// No-op implementation - does not publish events anywhere.
    /// </summary>
    public Task PublishEventAsync(TaskChangeEventDto eventDto, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "RabbitMQ is disabled - skipping event publication: TaskId={TaskId}, EventType={EventType}",
            eventDto?.TaskId,
            eventDto?.EventType);
        
        return Task.CompletedTask;
    }
}

