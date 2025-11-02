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
 
 *   The above copyright notice and this permission notice shall be included in all
 *   copies or substantial portions of the Software.
 
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 */

namespace TaskForge.MessageConsumer;

/// <summary>
/// Worker service that hosts MassTransit bus for consuming messages.
/// MassTransit automatically manages the bus lifecycle and message consumption.
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // MassTransit bus is managed automatically as a hosted service
        // We just need to keep the worker alive
        _logger.LogInformation("🚀 MessageConsumer Worker started. MassTransit bus will consume messages automatically.");
        _logger.LogInformation("📋 Listening for task change events from RabbitMQ queue: 'task-change-events'");
        _logger.LogInformation("💡 Worker is running and ready to process messages...");
        
        var heartbeatCounter = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            
            heartbeatCounter++;
            if (heartbeatCounter % 5 == 0) // Every 5 minutes
            {
                _logger.LogInformation("💓 MessageConsumer Worker heartbeat: Still running and processing messages... (Running for {Minutes} minutes)", heartbeatCounter);
            }
        }
        
        _logger.LogInformation("🛑 MessageConsumer Worker is stopping. Shutting down gracefully...");
    }
}
