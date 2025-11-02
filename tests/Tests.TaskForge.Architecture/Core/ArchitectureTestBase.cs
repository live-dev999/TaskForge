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
using NetArchTest.Rules;
using FluentAssertions;

namespace Tests.TaskForge.Architecture.Core;

/// <summary>
/// Base class for all architecture tests.
/// Provides common functionality and assembly resolution.
/// </summary>
public abstract class ArchitectureTestBase
{
    /// <summary>
    /// Gets the assemblies to test. Override this in derived classes to provide project-specific assemblies.
    /// </summary>
    protected abstract Dictionary<string, Assembly> GetAssemblies();

    /// <summary>
    /// Gets the assembly resolver instance.
    /// </summary>
    protected AssemblyResolver AssemblyResolver { get; }

    /// <summary>
    /// Gets the source code analyzer instance.
    /// </summary>
    protected SourceCodeAnalyzer SourceAnalyzer { get; }

    /// <summary>
    /// Gets the dependency graph analyzer instance.
    /// </summary>
    protected DependencyGraphAnalyzer DependencyAnalyzer { get; }

    /// <summary>
    /// Gets the test configuration.
    /// </summary>
    protected TestConfiguration Configuration { get; }

    protected ArchitectureTestBase()
    {
        Configuration = new TestConfiguration();
        AssemblyResolver = new AssemblyResolver(GetAssemblies(), Configuration);
        SourceAnalyzer = new SourceCodeAnalyzer(AssemblyResolver);
        DependencyAnalyzer = new DependencyGraphAnalyzer(AssemblyResolver);
    }

    /// <summary>
    /// Gets a specific assembly by name.
    /// </summary>
    protected Assembly GetAssembly(string name) => AssemblyResolver.GetAssembly(name);

    /// <summary>
    /// Gets all production assemblies (excludes test assemblies).
    /// </summary>
    protected IEnumerable<Assembly> GetProductionAssemblies() => AssemblyResolver.GetProductionAssemblies();

    /// <summary>
    /// Gets source files from an assembly.
    /// </summary>
    protected List<string> GetSourceFiles(Assembly assembly) => SourceAnalyzer.GetSourceFiles(assembly);

    /// <summary>
    /// Checks if a position in source code is within a comment.
    /// </summary>
    protected bool IsInComment(string content, int position) => SourceAnalyzer.IsInComment(content, position);

    /// <summary>
    /// Gets the method context at a given position.
    /// </summary>
    protected string GetMethodContext(string content, int position) => SourceAnalyzer.GetMethodContext(content, position);

    /// <summary>
    /// Finds circular dependencies in the dependency graph.
    /// </summary>
    protected List<List<string>> FindCircularDependencies() => DependencyAnalyzer.FindCycles();

    /// <summary>
    /// Validates layer dependencies according to Clean Architecture rules.
    /// </summary>
    protected void ValidateLayerDependencies(Dictionary<string, List<string>> allowedDependencies)
    {
        var violations = DependencyAnalyzer.ValidateLayerDependencies(allowedDependencies);
        violations.Should().BeEmpty(
            $"Layer dependency violations found:\n{string.Join("\n", violations)}");
    }

    /// <summary>
    /// Gets the full assembly name for a layer using configuration.
    /// </summary>
    protected string GetAssemblyName(string layerName) => Configuration.GetAssemblyName(layerName);
}

