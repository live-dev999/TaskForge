/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using Microsoft.Extensions.Hosting;
using Moq;

namespace Tests.TaskForge.Core.Helpers;

/// <summary>
/// Helper methods for creating environment instances for testing
/// </summary>
public static class EnvironmentTestHelper
{
    /// <summary>
    /// Creates a Development environment mock
    /// </summary>
    /// <returns>IHostEnvironment configured as Development</returns>
    public static IHostEnvironment CreateDevelopmentEnvironment()
    {
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.SetupGet(e => e.EnvironmentName).Returns("Development");
        return mockEnvironment.Object;
    }

    /// <summary>
    /// Creates a Production environment mock
    /// </summary>
    /// <returns>IHostEnvironment configured as Production</returns>
    public static IHostEnvironment CreateProductionEnvironment()
    {
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.SetupGet(e => e.EnvironmentName).Returns("Production");
        return mockEnvironment.Object;
    }

    /// <summary>
    /// Creates a custom environment mock
    /// </summary>
    /// <param name="environmentName">Environment name</param>
    /// <returns>IHostEnvironment with specified environment name</returns>
    public static IHostEnvironment CreateEnvironment(string environmentName)
    {
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment.SetupGet(e => e.EnvironmentName).Returns(environmentName);
        return mockEnvironment.Object;
    }
}

