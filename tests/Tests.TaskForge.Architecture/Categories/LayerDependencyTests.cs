/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using System.Reflection;
using System.Linq;
using FluentAssertions;
using NetArchTest.Rules;
using Tests.TaskForge.Architecture.Core;

namespace Tests.TaskForge.Architecture.Categories;

/// <summary>
/// Tests for layer dependency rules (Clean Architecture).
/// </summary>
public class LayerDependencyTests : ArchitectureTestBase
{
    protected override Dictionary<string, Assembly> GetAssemblies()
    {
        return new Dictionary<string, Assembly>
        {
            { "API", typeof(TaskForge.API.Controllers.TaskItemsController).Assembly },
            { "Application", typeof(TaskForge.Application.Core.Result<>).Assembly },
            { "Domain", typeof(TaskForge.Domain.TaskItem).Assembly },
            { "Persistence", typeof(TaskForge.Persistence.DataContext).Assembly },
            { "EventProcessor", typeof(TaskForge.EventProcessor.Controllers.EventsController).Assembly },
            { "MessageConsumer", typeof(TaskForge.MessageConsumer.Consumers.TaskChangeEventConsumer).Assembly }
        };
    }

    [Fact]
    public void Domain_Should_Not_Have_Dependencies_On_Other_Layers()
    {
        var domainAssembly = GetAssembly("Domain");
        
        // Use configuration to build dependency names dynamically
        var forbiddenDependencies = Configuration.LayerNames
            .Where(layer => layer != "Domain")
            .Select(layer => GetAssemblyName(layer))
            .ToArray();
        
        // NetArchTest requires building the chain properly
        var result = Types
            .InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenDependencies)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"Domain layer should not depend on other layers. Violations: {string.Join(", ", result.FailingTypes)}");
    }

    [Fact]
    public void Application_Should_Not_Have_Dependencies_On_API_Or_Infrastructure()
    {
        var applicationAssembly = GetAssembly("Application");
        
        // Use configuration for forbidden dependencies
        var forbiddenLayers = new[] { "API", "EventProcessor", "MessageConsumer" };
        var forbiddenDependencies = forbiddenLayers
            .Select(layer => GetAssemblyName(layer))
            .ToArray();
        
        var result = Types
            .InAssembly(applicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenDependencies)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"Application layer should not depend on API or infrastructure layers. Violations: {string.Join(", ", result.FailingTypes)}");
    }

    [Fact]
    public void Should_Not_Have_Circular_Dependencies()
    {
        var cycles = FindCircularDependencies();
        
        cycles.Should().BeEmpty(
            $"Should not have circular dependencies between assemblies. Found cycles:\n{string.Join("\n", cycles.Select(c => string.Join(" -> ", c)))}");
    }

    [Fact]
    public void Validate_Clean_Architecture_Layer_Dependencies()
    {
        // Define allowed dependencies according to Clean Architecture
        var allowedDependencies = new Dictionary<string, List<string>>
        {
            { "Domain", new List<string>() }, // Domain depends on nothing
            { "Application", new List<string> { "Domain" } }, // Application depends only on Domain
            { "Persistence", new List<string> { "Domain" } }, // Persistence depends only on Domain
            { "API", new List<string> { "Application", "Persistence", "Domain" } }, // API depends on Application/Persistence/Domain
            { "EventProcessor", new List<string> { "Domain" } }, // EventProcessor depends only on Domain
            { "MessageConsumer", new List<string> { "Domain" } } // MessageConsumer depends only on Domain
        };

        ValidateLayerDependencies(allowedDependencies);
    }
}

