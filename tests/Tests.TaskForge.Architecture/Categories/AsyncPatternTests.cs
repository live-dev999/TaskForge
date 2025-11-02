/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using FluentAssertions;
using Tests.TaskForge.Architecture.Core;
using Tests.TaskForge.Architecture.Helpers;

namespace Tests.TaskForge.Architecture.Categories;

/// <summary>
/// Tests for async/await patterns and ConfigureAwait usage.
/// </summary>
public class AsyncPatternTests : ArchitectureTestBase
{
    protected override Dictionary<string, Assembly> GetAssemblies()
    {
        return new Dictionary<string, Assembly>
        {
            { "Application", typeof(TaskForge.Application.Core.Result<>).Assembly },
            { "Persistence", typeof(TaskForge.Persistence.DataContext).Assembly }
        };
    }

    [Fact]
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

    [Fact]
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

