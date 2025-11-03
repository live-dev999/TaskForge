/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using Microsoft.AspNetCore.Http;

namespace Tests.TaskForge.Core.Helpers;

/// <summary>
/// Helper methods for creating HttpContext instances for testing
/// </summary>
public static class HttpContextTestHelper
{
    /// <summary>
    /// Creates a basic HttpContext with memory stream for response body
    /// </summary>
    /// <returns>HttpContext instance</returns>
    public static HttpContext CreateHttpContext()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        return httpContext;
    }

    /// <summary>
    /// Creates an HttpContext with a service provider
    /// </summary>
    /// <param name="serviceProvider">Service provider to use</param>
    /// <returns>HttpContext with configured service provider</returns>
    public static HttpContext CreateHttpContext(IServiceProvider serviceProvider)
    {
        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };
        httpContext.Response.Body = new MemoryStream();
        return httpContext;
    }
}

