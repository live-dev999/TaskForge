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

using MassTransit;
using Microsoft.Extensions.Logging;
using TaskForge.MessageConsumer;
using TaskForge.MessageConsumer.Consumers;

var builder = Host.CreateApplicationBuilder(args);

var logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger<Program>();

logger.LogInformation("[START] Starting TaskForge.MessageConsumer service...");

// Get RabbitMQ configuration
var configuration = builder.Configuration;
var host = configuration["RabbitMQ:HostName"] ?? "localhost";
var port = configuration.GetValue<int>("RabbitMQ:Port", 5672);
var userName = configuration["RabbitMQ:UserName"] ?? "guest";
var password = configuration["RabbitMQ:Password"] ?? "guest";

logger.LogInformation(
    "[CONFIG] Configuring RabbitMQ connection: Host={Host}, Port={Port}, User={UserName}",
    host,
    port,
    userName);

// Configure MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    logger.LogInformation("[CONFIG] Configuring MassTransit...");

    // Configure message endpoint naming convention
    // This ensures consistent endpoint names across services
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("TaskForge", false));
    logger.LogInformation("[OK] Endpoint name formatter configured: KebabCase with prefix 'TaskForge'");

    // Add consumer
    x.AddConsumer<TaskChangeEventConsumer>();
    logger.LogInformation("[OK] TaskChangeEventConsumer registered");

    // Configure RabbitMQ
    x.UsingRabbitMq((context, cfg) =>
    {
        logger.LogInformation("[CONNECT] Connecting to RabbitMQ: Host={Host}, Port={Port}", host, port);

        cfg.Host(host, (ushort)port, "/", h =>
        {
            h.Username(userName);
            h.Password(password);
            // Configure connection timeout - MassTransit has built-in retry for connections
            h.RequestedConnectionTimeout(TimeSpan.FromSeconds(30));
        });

        logger.LogInformation("[OK] RabbitMQ host configured (MassTransit has built-in connection retry)");

        // Configure receive endpoint for TaskChangeEventDto
        cfg.ReceiveEndpoint("task-change-events", e =>
        {
            logger.LogInformation("[ENDPOINT] Configuring receive endpoint: 'task-change-events'");

            e.ConfigureConsumer<TaskChangeEventConsumer>(context);
            logger.LogInformation("[OK] Consumer configured for endpoint");

            // Configure retry policy
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            logger.LogInformation("[OK] Retry policy configured: 3 retries with 5 second intervals");

            logger.LogInformation("[OK] Receive endpoint 'task-change-events' fully configured");
        });

        cfg.ConfigureEndpoints(context);
        logger.LogInformation("[OK] All endpoints configured");
    });
});

logger.LogInformation("[OK] MassTransit configuration completed");

// Add hosted service
builder.Services.AddHostedService<Worker>();
logger.LogInformation("[OK] Worker hosted service registered");

var hostApp = builder.Build();

logger.LogInformation("[START] Starting host application...");
logger.LogInformation("[LISTEN] Listening for messages on queue: 'task-change-events'");
logger.LogInformation("RabbitMQ Management UI: http://{Host}:15672", host);

hostApp.Run();
