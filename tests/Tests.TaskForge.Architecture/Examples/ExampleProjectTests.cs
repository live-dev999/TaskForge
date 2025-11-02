/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using FluentAssertions;
using Tests.TaskForge.Architecture.Core;
using Tests.TaskForge.Architecture.Helpers;

namespace Tests.TaskForge.Architecture.Examples;

/// <summary>
/// Example of how to use the refactored architecture tests in another project.
/// This demonstrates how to create reusable architecture tests.
/// </summary>
public class ExampleProjectTests : ArchitectureTestBase
{
    // Step 1: Define your project's assemblies
    protected override Dictionary<string, Assembly> GetAssemblies()
    {
        // Replace these with your actual project types
        return new Dictionary<string, Assembly>
        {
            { "API", typeof(TaskForge.API.Controllers.TaskItemsController).Assembly },
            { "Application", typeof(TaskForge.Application.Core.Result<>).Assembly },
            { "Domain", typeof(TaskForge.Domain.TaskItem).Assembly },
            { "Persistence", typeof(TaskForge.Persistence.DataContext).Assembly }
        };
    }

    // Step 2: Optionally configure test settings by overriding Configuration property
    // Note: Configuration is already initialized in base class, but you can modify it
    // For example, in constructor or test setup:
    protected void ConfigureTest()
    {
        Configuration.ProjectPrefix = "YourProject";
        Configuration.LayerNames = new List<string> { "API", "Application", "Domain", "Persistence" };
        Configuration.ExcludePatterns.Add("Generated");
    }

    // Step 3: Write your tests using the base class functionality
    
    [Fact]
    public void Example_Layer_Dependency_Test()
    {
        var domainAssembly = GetAssembly("Domain");
        
        // Use NetArchTest for layer validation
        var result = NetArchTest.Rules.Types
            .InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOn("YourProject.Application")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Example_Async_Pattern_Test()
    {
        var applicationAssembly = GetAssembly("Application");
        var sourceFiles = GetSourceFiles(applicationAssembly);
        
        // Use helpers for common checks
        var violations = AsyncPatternHelper.FindMissingConfigureAwait(
            SourceAnalyzer, 
            sourceFiles, 
            Configuration
        );

        violations.Should().BeEmpty();
    }

    [Fact]
    public void Example_Naming_Convention_Test()
    {
        var assemblies = GetProductionAssemblies();
        
        // Use naming convention helpers
        var violations = NamingConventionHelper.FindPrivateFieldsWithoutUnderscore(assemblies);
        
        violations.Should().BeEmpty();
    }

    [Fact]
    public void Example_Circular_Dependency_Test()
    {
        // Use dependency analyzer
        var cycles = FindCircularDependencies();
        
        cycles.Should().BeEmpty();
    }

    [Fact]
    public void Example_Custom_Source_Code_Check()
    {
        var applicationAssembly = GetAssembly("Application");
        var sourceFiles = GetSourceFiles(applicationAssembly);
        
        var violations = new List<string>();

        foreach (var sourceFile in sourceFiles)
        {
            if (!File.Exists(sourceFile)) continue;

            var content = File.ReadAllText(sourceFile);
            
            // Use SourceAnalyzer helper methods
            if (SourceAnalyzer.IsInComment(content, 100))
            {
                // Skip commented code
                continue;
            }

            // Your custom checks here
            if (content.Contains("TODO"))
            {
                violations.Add($"{Path.GetFileName(sourceFile)} - Contains TODO");
            }
        }

        violations.Should().BeEmpty();
    }
}

