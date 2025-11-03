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

using TaskForge.MessageConsumer.Models;

namespace TaskForge.MessageConsumer.Services;

/// <summary>
/// Interface for consuming task change events from RabbitMQ message queue.
/// </summary>
public interface IMessageConsumerService
{
    /// <summary>
    /// Starts consuming messages from the RabbitMQ queue.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to stop consuming.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task StartConsumingAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Processes a received task change event message.
    /// </summary>
    /// <param name="eventData">The task change event data to process.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ProcessMessageAsync(TaskChangeEvent eventData);

    /// <summary>
    /// Stops consuming messages from the RabbitMQ queue.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task StopConsumingAsync(CancellationToken cancellationToken);
}

