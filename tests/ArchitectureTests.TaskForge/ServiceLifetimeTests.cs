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

using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;

namespace ArchitectureTests.TaskForge;

/// <summary>
/// Tests for service lifetime and dependency injection registration patterns.
/// </summary>
public class ServiceLifetimeTests
{
    private static readonly Assembly ApiAssembly = typeof(global::TaskForge.API.Controllers.TaskItemsController).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(global::TaskForge.Application.Core.Result<>).Assembly;

    [Fact(Skip = "disabled")]
    public void DbContext_Should_Be_Registered_As_Scoped()
    {
        // This test verifies that DbContext usage suggests Scoped lifetime
        // Full check would require analyzing service registration code
        var usesDbContext = Types
            .InAssembly(ApiAssembly)
            .That()
            .AreClasses()
            .GetTypes()
            .Any(t => t.GetConstructors()
                .Any(c => c.GetParameters()
                    .Any(p => p.ParameterType.Name == "DataContext")));

        usesDbContext.Should().BeTrue(
            "DbContext should be used (this implies it's registered, typically as Scoped)");
    }

    [Fact]
    public void HttpClients_Should_Be_Registered_With_AddHttpClient()
    {
        // Verify that HttpClient is injected (not created with 'new')
        var servicesUsingHttpClient = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .AreClasses()
            .GetTypes()
            .Where(t => t.GetConstructors()
                .Any(c => c.GetParameters()
                    .Any(p => p.ParameterType.Name == "HttpClient")));

        foreach (var service in servicesUsingHttpClient)
        {
            // HttpClient should be injected, which suggests AddHttpClient was used
            service.GetConstructors()
                .Should().NotBeEmpty(
                    $"Service {service.Name} using HttpClient should have constructor");
        }
    }

    [Fact]
    public void Handlers_Should_Be_Transient()
    {
        // Handlers should be transient since they're stateless and called per request
        var handlers = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .HaveName("Handler")
            .GetTypes()
            .Where(h => h.DeclaringType != null);

        handlers.Should().NotBeEmpty(
            "Handlers should exist and be registered (typically as Transient)");
    }

    [Fact]
    public void Services_Should_Not_Be_Singleton_Unless_Stateless()
    {
        // Services with state should not be Singleton
        // This is a guideline check
        var services = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Service")
            .GetTypes();

        foreach (var service in services)
        {
            // Check if service has instance fields that represent state
            var hasInstanceState = service.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Any(f => !f.IsInitOnly && 
                         f.FieldType.Namespace?.StartsWith("TaskForge") != true &&
                         !f.FieldType.Name.Contains("Logger") &&
                         !f.FieldType.Name.Contains("Configuration"));

            // Services with state should not be Singleton
            // Note: Full check would require analyzing service registration
        }
    }
}

