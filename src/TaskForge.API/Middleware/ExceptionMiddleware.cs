using System.Net;
using System.Text.Json;
using TaskForge.Application.Core;

namespace TaskForge.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment environment
    )
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            
            // Log inner exception if present
            if (ex.InnerException != null)
            {
                _logger.LogError(ex.InnerException, "Inner exception: {Message}", ex.InnerException.Message);
            }
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var details = ex.StackTrace?.ToString();
            if (ex.InnerException != null)
            {
                details = $"{ex.Message}\nInner Exception: {ex.InnerException.Message}\n\n{details}";
            }

            var response = _environment.IsDevelopment()
                ? new AppException(
                    context.Response.StatusCode,
                    ex.Message,
                    details
                )
                : new AppException(context.Response.StatusCode, "Internal Server Error");

            var option = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, option);

            await context.Response.WriteAsync(json);
        }
    }
}
