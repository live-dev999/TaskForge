/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using MediatR;
using Moq;
using TaskForge.Application.Core;

namespace Tests.TaskForge.Core.Helpers;

/// <summary>
/// Helper methods for creating mediator mocks for testing
/// </summary>
public static class MediatorTestHelper
{
    /// <summary>
    /// Creates a mock IMediator that returns success for any request
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="response">Response value to return</param>
    /// <returns>Mock IMediator configured to return success</returns>
    public static Mock<IMediator> CreateMediatorMock<TRequest, TResponse>(TResponse response)
        where TRequest : IRequest<Result<TResponse>>
    {
        var mockMediator = new Mock<IMediator>();
        mockMediator
            .Setup(m => m.Send(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TResponse>.Success(response));
        return mockMediator;
    }

    /// <summary>
    /// Creates a mock IMediator that returns failure for any request
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="errorMessage">Error message to return</param>
    /// <returns>Mock IMediator configured to return failure</returns>
    public static Mock<IMediator> CreateMediatorMockWithFailure<TRequest, TResponse>(string errorMessage)
        where TRequest : IRequest<Result<TResponse>>
    {
        var mockMediator = new Mock<IMediator>();
        mockMediator
            .Setup(m => m.Send(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TResponse>.Failure(errorMessage));
        return mockMediator;
    }

    /// <summary>
    /// Creates a basic mock IMediator for custom setup
    /// </summary>
    /// <returns>Mock IMediator instance</returns>
    public static Mock<IMediator> CreateMediatorMock()
    {
        return new Mock<IMediator>();
    }

    /// <summary>
    /// Creates a mock IMediator with custom setup action
    /// </summary>
    /// <param name="setup">Action to configure the mock</param>
    /// <returns>Configured mock IMediator</returns>
    public static Mock<IMediator> CreateMediatorMock(Action<Mock<IMediator>> setup)
    {
        var mock = new Mock<IMediator>();
        setup(mock);
        return mock;
    }
}

