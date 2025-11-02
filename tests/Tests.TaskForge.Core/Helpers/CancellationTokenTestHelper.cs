/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

namespace Tests.TaskForge.Core.Helpers;

/// <summary>
/// Helper methods for creating CancellationToken instances for testing
/// </summary>
public static class CancellationTokenTestHelper
{
    /// <summary>
    /// Creates a cancelled CancellationToken for testing cancellation scenarios
    /// </summary>
    /// <returns>Cancelled CancellationToken</returns>
    public static CancellationToken CreateCancelledToken()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        return cancellationTokenSource.Token;
    }

    /// <summary>
    /// Creates a CancellationTokenSource that is already cancelled
    /// </summary>
    /// <returns>Cancelled CancellationTokenSource</returns>
    public static CancellationTokenSource CreateCancelledTokenSource()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        return cancellationTokenSource;
    }

    /// <summary>
    /// Returns the default CancellationToken.None for non-cancellation scenarios
    /// </summary>
    /// <returns>CancellationToken.None</returns>
    public static CancellationToken None() => CancellationToken.None;
}

