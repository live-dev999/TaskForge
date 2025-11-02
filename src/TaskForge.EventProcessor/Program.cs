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

using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register event logging service
builder.Services.AddSingleton<TaskForge.EventProcessor.Services.IEventLogger, TaskForge.EventProcessor.Services.EventLogger>();

// Add OpenTelemetry tracing
var serviceName = builder.Configuration["OpenTelemetry:ServiceName"] 
    ?? builder.Configuration["OTEL_SERVICE_NAME"] 
    ?? "TaskForge.EventProcessor";
var serviceVersion = builder.Configuration["OpenTelemetry:ServiceVersion"] 
    ?? builder.Configuration["OTEL_SERVICE_VERSION"] 
    ?? "1.0.0";
var enableConsoleExporter = builder.Configuration.GetValue<bool>("OpenTelemetry:EnableConsoleExporter", 
    builder.Configuration.GetValue<bool>("OTEL_ENABLE_CONSOLE_EXPORTER", true));
var otlpEndpoint = builder.Configuration["OpenTelemetry:Otlp:Endpoint"] 
    ?? builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];

builder.Services.AddOpenTelemetry()
    .WithTracing(tracingBuilder =>
    {
        tracingBuilder
            .AddSource(serviceName)
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
                    .AddAttributes(new Dictionary<string, object>
                    {
                        ["deployment.environment"] = builder.Configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development"
                    }))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();

        if (enableConsoleExporter)
        {
            tracingBuilder.AddConsoleExporter();
        }

        if (!string.IsNullOrEmpty(otlpEndpoint))
        {
            tracingBuilder.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otlpEndpoint);
            });
        }
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
