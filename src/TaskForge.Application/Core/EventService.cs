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

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

#nullable disable

namespace TaskForge.Application.Core;

/// <summary>
/// Service for sending task change events to EventProcessor service via HTTP.
/// </summary>
public class EventService : IEventService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EventService> _logger;
    private readonly string _eventProcessorBaseUrl;

    public EventService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<EventService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        // Get EventProcessor URL from configuration
        // Priority: 1) appsettings.json "EventProcessor:BaseUrl"
        //           2) Environment variable "EVENT_PROCESSOR_BASE_URL"
        //           3) Default: null (disabled)
        _eventProcessorBaseUrl = configuration["EventProcessor:BaseUrl"]
            ?? configuration["EVENT_PROCESSOR_BASE_URL"];

        if (!string.IsNullOrEmpty(_eventProcessorBaseUrl))
        {
            _httpClient.BaseAddress = new Uri(_eventProcessorBaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(5); // Short timeout to avoid blocking
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }
    }

    /// <summary>
    /// Sends a task change event to the EventProcessor service.
    /// Handles errors gracefully - if EventProcessor is unavailable, logs the error but doesn't throw.
    /// </summary>
    public async Task<bool> SendEventAsync(TaskChangeEventDto eventDto, CancellationToken cancellationToken = default)
    {
        // If EventProcessor URL is not configured, skip sending
        if (string.IsNullOrEmpty(_eventProcessorBaseUrl))
        {
            _logger.LogDebug("EventProcessor URL not configured, skipping event send for TaskId: {TaskId}", eventDto.TaskId);
            return false;
        }

        if (eventDto == null)
        {
            _logger.LogWarning("Attempted to send null event");
            return false;
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/api/events",
                eventDto,
                cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "Successfully sent task event to EventProcessor: TaskId={TaskId}, EventType={EventType}",
                    eventDto.TaskId,
                    eventDto.EventType);
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogWarning(
                    "EventProcessor returned non-success status: {StatusCode} for TaskId={TaskId}, EventType={EventType}. Response: {Response}",
                    response.StatusCode,
                    eventDto.TaskId,
                    eventDto.EventType,
                    errorContent);
                return false;
            }
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogWarning(
                "Timeout while sending event to EventProcessor: TaskId={TaskId}, EventType={EventType}",
                eventDto.TaskId,
                eventDto.EventType);
            return false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to send event to EventProcessor (service may be unavailable): TaskId={TaskId}, EventType={EventType}",
                eventDto.TaskId,
                eventDto.EventType);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while sending event to EventProcessor: TaskId={TaskId}, EventType={EventType}",
                eventDto.TaskId,
                eventDto.EventType);
            return false;
        }
    }
}

