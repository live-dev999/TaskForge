/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using Microsoft.EntityFrameworkCore;
using TaskForge.Persistence;

namespace Tests.TaskForge.Core.Helpers;

/// <summary>
/// Helper methods for creating test database contexts
/// </summary>
public static class DatabaseTestHelper
{
    /// <summary>
    /// Creates an in-memory DataContext for testing
    /// </summary>
    /// <param name="databaseName">Optional database name. If null, a new GUID will be used</param>
    /// <returns>In-memory DataContext instance</returns>
    public static DataContext CreateInMemoryContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new DataContext(options);
    }

    /// <summary>
    /// Creates an in-memory DataContext with a unique database name for isolation
    /// </summary>
    /// <returns>In-memory DataContext instance with unique database name</returns>
    public static DataContext CreateIsolatedInMemoryContext()
    {
        return CreateInMemoryContext();
    }
}

