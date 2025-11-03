/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using System.Reflection;
using System.Linq;
using FluentAssertions;
using ArchitectureTests.TaskForge.Core;
using ArchitectureTests.TaskForge.Helpers;

namespace ArchitectureTests.TaskForge.Categories;

/// <summary>
/// Tests for async/await patterns and ConfigureAwait usage.
/// </summary>
public class AsyncPatternTests : ArchitectureTestBase
{
    protected override Dictionary<string, Assembly> GetAssemblies()
    {
        return new Dictionary<string, Assembly>
        {
            { "Application", typeof(global::TaskForge.Application.Core.Result<>).Assembly },
            { "Persistence", typeof(global::TaskForge.Persistence.DataContext).Assembly }
        };
    }

    [Fact(Skip = "Temporarily disabled - ConfigureAwait(false) patterns need review")]
    public void Library_Code_Should_Use_ConfigureAwait_False()
    {
        var violations = new List<Violation>();
        var libraryAssemblies = new[] { GetAssembly("Application"), GetAssembly("Persistence") };

        foreach (var assembly in libraryAssemblies)
        {
            var sourceFiles = GetSourceFiles(assembly);
            violations.AddRange(AsyncPatternHelper.FindMissingConfigureAwait(SourceAnalyzer, sourceFiles, Configuration));
        }

        violations.Should().BeEmpty(
            $"Library code should use ConfigureAwait(false) to prevent deadlocks. Violations:\n{string.Join("\n", violations.Take(15).Select(v => v.ToString()))}");
    }

    [Fact(Skip = "Temporarily disabled - requires source code analysis")]
    public void Async_Methods_Should_Not_Use_Blocking_Calls()
    {
        var violations = new List<Violation>();
        var assemblies = GetProductionAssemblies();

        foreach (var assembly in assemblies)
        {
            var sourceFiles = GetSourceFiles(assembly);
            violations.AddRange(AsyncPatternHelper.FindBlockingCalls(SourceAnalyzer, sourceFiles, Configuration));
        }

        violations.Should().BeEmpty(
            $"Async methods should not use blocking calls. Violations:\n{string.Join("\n", violations.Take(10).Select(v => v.ToString()))}");
    }
}

