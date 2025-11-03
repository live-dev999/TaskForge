/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TaskForge.Domain;
using Tests.TaskForge.Core.Helpers;

namespace Tests.TaskForge.Core.Fixtures;

/// <summary>
/// Base fixture for controller tests that provides common setup and helper methods
/// </summary>
public abstract class BaseControllerTestFixture
{
    /// <summary>
    /// Creates a controller with a mocked IMediator
    /// </summary>
    /// <typeparam name="TController">Controller type</typeparam>
    /// <param name="mediator">Mediator instance (usually a mock)</param>
    /// <returns>Controller instance with configured HttpContext</returns>
    protected TController CreateController<TController>(IMediator mediator) where TController : ControllerBase, new()
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton(mediator)
            .BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        var controller = new TController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        return controller;
    }

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

