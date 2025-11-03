/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.TaskForge.Core.Helpers;

/// <summary>
/// Helper methods for creating logger instances for testing
/// </summary>
public static class LoggerTestHelper
{
    /// <summary>
    /// Creates a mock logger for the specified type
    /// </summary>
    /// <typeparam name="T">Type to create logger for</typeparam>
    /// <returns>Mock logger instance</returns>
    public static ILogger<T> CreateLogger<T>()
    {
        return new Mock<ILogger<T>>().Object;
    }

    /// <summary>
    /// Creates a mock logger that can be verified (for testing logging calls)
    /// </summary>
    /// <typeparam name="T">Type to create logger for</typeparam>
    /// <returns>Mock logger that can be verified</returns>
    public static Mock<ILogger<T>> CreateVerifiableLogger<T>()
    {
        return new Mock<ILogger<T>>();
    }

    /// <summary>
    /// Creates a logger with specific behavior for testing
    /// </summary>
    /// <typeparam name="T">Type to create logger for</typeparam>
    /// <param name="setup">Action to configure the mock</param>
    /// <returns>Configured mock logger</returns>
    public static Mock<ILogger<T>> CreateLogger<T>(Action<Mock<ILogger<T>>> setup)
    {
        var mock = new Mock<ILogger<T>>();
        setup(mock);
        return mock;
    }
}

