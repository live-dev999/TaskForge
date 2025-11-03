/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.

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

using Microsoft.EntityFrameworkCore;
using TaskForge.Application.Core;
using TaskForge.Application.TaskItems;
using MediatR;
using FluentValidation;
using FluentValidation.AspNetCore;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using MassTransit;

namespace TaskForge.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddDbContext<Persistence.DataContext>(opt =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection") 
                ?? config["ConnectionString"] 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            opt.UseNpgsql(connectionString);
        });
        services.AddCors(opt =>
        {
            // Get allowed origins from configuration
            // Priority: 1) appsettings.json "Cors:AllowedOrigins" array
            //           2) Environment variable "CORS_ALLOWED_ORIGINS" (semicolon-separated, e.g., "http://localhost:3000;http://localhost:3001")
            //           3) Default: "http://localhost:3000"
            var allowedOrigins = config.GetSection("Cors:AllowedOrigins").Get<string[]>();
            
            // Fallback to environment variable if not in config (useful for Docker)
            if (allowedOrigins == null || allowedOrigins.Length == 0)
            {
                var corsOriginsEnv = config["CORS_ALLOWED_ORIGINS"];
                if (!string.IsNullOrEmpty(corsOriginsEnv))
                {
                    // Split by semicolon to support multiple origins
                    allowedOrigins = corsOriginsEnv.Split(';', StringSplitOptions.RemoveEmptyEntries);
                }
            }
            
            // Default to localhost:3000 if nothing is configured
            if (allowedOrigins == null || allowedOrigins.Length == 0)
            {
                allowedOrigins = new[] { "http://localhost:3000" };
            }
            
            opt.AddPolicy(
                "CorsPolicy",
                policy =>
                {
                    policy
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins(allowedOrigins);
                }
            );
        });

        services.AddMediatR(typeof(List.Handler));
        services.AddAutoMapper(typeof(MappingProfiles).Assembly);
        
        // Configure FluentValidation
        services.AddValidatorsFromAssemblyContaining<Create>();
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        
        // Add Health Checks
        services.AddHealthChecks()
            .AddDbContextCheck<Persistence.DataContext>(
                name: "database",
                tags: new[] { "ready" });
        
        // Add HttpClient for EventService
        services.AddHttpClient<TaskForge.Application.Core.EventService>();
        services.AddScoped<TaskForge.Application.Core.IEventService>(sp =>
            sp.GetRequiredService<TaskForge.Application.Core.EventService>());
        
        // Configure RabbitMQ/MassTransit (optional - can be disabled for local development)
        var rabbitMQEnabled = config.GetValue<bool>("RabbitMQ:Enabled", true);
        
        if (rabbitMQEnabled)
        {
            // Configure MassTransit with RabbitMQ
            services.AddMassTransit(x =>
            {
                // Configure message endpoint naming convention
                // This ensures consistent endpoint names across services
                x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("TaskForge", false));

                // Configure RabbitMQ
                x.UsingRabbitMq((context, cfg) =>
                {
                    var host = config["RabbitMQ:HostName"] ?? "localhost";
                    var port = config.GetValue<int>("RabbitMQ:Port", 5672);
                    var userName = config["RabbitMQ:UserName"] ?? "guest";
                    var password = config["RabbitMQ:Password"] ?? "guest";

                    cfg.Host(host, (ushort)port, "/", h =>
                    {
                        h.Username(userName);
                        h.Password(password);
                        // Configure connection timeout - MassTransit has built-in retry for connections
                        h.RequestedConnectionTimeout(TimeSpan.FromSeconds(30));
                    });

                    // Configure publish endpoint for TaskChangeEventDto
                    cfg.Message<TaskForge.Application.Core.TaskChangeEventDto>(e =>
                    {
                        e.SetEntityName("task-change-events");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            // Register real MessageProducer
            services.AddScoped<TaskForge.Application.Core.IMessageProducer, TaskForge.Application.Core.MessageProducer>();
        }
        else
        {
            // Register null MessageProducer (no-op implementation)
            services.AddScoped<TaskForge.Application.Core.IMessageProducer, TaskForge.Application.Core.NullMessageProducer>();
        }
        
        // Add OpenTelemetry tracing
        AddOpenTelemetryTracing(services, config);
        
        return services;
    }
    
    private static void AddOpenTelemetryTracing(IServiceCollection services, IConfiguration config)
    {
        // Read configuration from appsettings.json or environment variables
        // Environment variables can use double underscore (__) for nested config: OpenTelemetry__ServiceName
        // Or standard OTEL_ prefixed env vars: OTEL_SERVICE_NAME
        var serviceName = config["OpenTelemetry:ServiceName"] 
            ?? config["OTEL_SERVICE_NAME"] 
            ?? "TaskForge.API";
        var serviceVersion = config["OpenTelemetry:ServiceVersion"] 
            ?? config["OTEL_SERVICE_VERSION"] 
            ?? "1.0.0";
        var enableConsoleExporter = config.GetValue<bool>("OpenTelemetry:EnableConsoleExporter", 
            config.GetValue<bool>("OTEL_ENABLE_CONSOLE_EXPORTER", true));
        var otlpEndpoint = config["OpenTelemetry:Otlp:Endpoint"] 
            ?? config["OTEL_EXPORTER_OTLP_ENDPOINT"];
        
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .AddSource(serviceName)
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
                            .AddAttributes(new Dictionary<string, object>
                            {
                                ["deployment.environment"] = config["ASPNETCORE_ENVIRONMENT"] ?? "Development"
                            }))
                    // Instrumentation for ASP.NET Core
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.EnrichWithHttpRequest = (activity, request) =>
                        {
                            activity.SetTag("http.request.path", request.Path);
                            activity.SetTag("http.request.method", request.Method);
                        };
                        options.EnrichWithHttpResponse = (activity, response) =>
                        {
                            activity.SetTag("http.response.status_code", response.StatusCode);
                        };
                        options.EnrichWithException = (activity, exception) =>
                        {
                            activity.SetTag("error.type", exception.GetType().Name);
                            activity.SetTag("error.message", exception.Message);
                        };
                    })
                    // Instrumentation for Entity Framework Core
                    .AddEntityFrameworkCoreInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                        options.EnrichWithIDbCommand = (activity, command) =>
                        {
                            activity.SetTag("db.statement", command.CommandText);
                        };
                    })
                    // Instrumentation for HTTP clients
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.EnrichWithHttpRequestMessage = (activity, request) =>
                        {
                            activity.SetTag("http.url", request.RequestUri?.ToString());
                            activity.SetTag("http.method", request.Method?.Method);
                        };
                        options.EnrichWithHttpResponseMessage = (activity, response) =>
                        {
                            activity.SetTag("http.status_code", (int)response.StatusCode);
                        };
                    });
                    // Note: AddRuntimeInstrumentation is for metrics, not tracing
                    // To add runtime metrics, use .WithMetrics() builder instead
                
                // Add exporters
                if (enableConsoleExporter)
                {
                    // Console exporter extension method from OpenTelemetry.Exporter.Console package
                    builder.AddConsoleExporter();
                }
                
                // Add OTLP exporter if endpoint is configured
                if (!string.IsNullOrEmpty(otlpEndpoint))
                {
                    // OTLP exporter extension method from OpenTelemetry.Exporter.OpenTelemetryProtocol package
                    builder.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                    });
                }
            });
    }
}
