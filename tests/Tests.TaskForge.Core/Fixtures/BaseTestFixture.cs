/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using TaskForge.Domain;
using Tests.TaskForge.Core.Helpers;

namespace Tests.TaskForge.Core.Fixtures;

/// <summary>
/// Base fixture for general unit tests that provides common helper methods
/// </summary>
public abstract class BaseTestFixture
{
    /// <summary>
    /// Creates a valid TaskItem for testing
    /// </summary>
    protected TaskItem CreateValidTaskItem(Action<TaskItem>? customize = null) => 
        TestDataFactory.CreateValidTaskItem(customize);

    /// <summary>
    /// Creates a cancelled CancellationToken for testing cancellation scenarios
    /// </summary>
    protected CancellationToken CreateCancelledToken() => 
        CancellationTokenTestHelper.CreateCancelledToken();
}

