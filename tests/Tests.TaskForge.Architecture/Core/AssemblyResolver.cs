/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using System.Reflection;

namespace Tests.TaskForge.Architecture.Core;

/// <summary>
/// Resolves and manages assemblies for architecture tests.
/// </summary>
public class AssemblyResolver
{
    private readonly Dictionary<string, Assembly> _assemblies;
    private readonly TestConfiguration? _configuration;

    public AssemblyResolver(Dictionary<string, Assembly> assemblies, TestConfiguration? configuration = null)
    {
        _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        _configuration = configuration;
    }

    /// <summary>
    /// Gets an assembly by its name/key.
    /// </summary>
    public Assembly GetAssembly(string name)
    {
        if (!_assemblies.TryGetValue(name, out var assembly))
        {
            throw new ArgumentException($"Assembly '{name}' not found. Available assemblies: {string.Join(", ", _assemblies.Keys)}");
        }
        return assembly;
    }

    /// <summary>
    /// Gets all assemblies.
    /// </summary>
    public Dictionary<string, Assembly> GetAllAssemblies() => _assemblies;

    /// <summary>
    /// Gets all production assemblies (excludes test assemblies).
    /// </summary>
    public IEnumerable<Assembly> GetProductionAssemblies()
    {
        return _assemblies.Values
            .Where(a => !a.GetName().Name?.Contains("Test") == true &&
                       !a.GetName().Name?.Contains("Tests") == true);
    }

    /// <summary>
    /// Gets assembly references for a given assembly.
    /// </summary>
    public List<string> GetAssemblyReferences(string assemblyName)
    {
        var assembly = GetAssembly(assemblyName);
        return assembly.GetReferencedAssemblies()
            .Select(a => a.Name ?? string.Empty)
            .Where(name => !string.IsNullOrEmpty(name))
            .ToList();
    }

    /// <summary>
    /// Builds a dependency graph of assemblies.
    /// </summary>
    public Dictionary<string, List<string>> BuildDependencyGraph()
    {
        var graph = new Dictionary<string, List<string>>();
        var projectPrefix = _configuration?.ProjectPrefix ?? "TaskForge";

        foreach (var (name, assembly) in _assemblies)
        {
            var refs = assembly.GetReferencedAssemblies()
                .Select(a => a.Name)
                .Where(a => a != null && 
                           (a.StartsWith(projectPrefix) || 
                            _assemblies.Keys.Any(k => a.Contains(ExtractProjectName(k)))))
                .Select(a => NormalizeAssemblyName(a!, projectPrefix))
                .Distinct()
                .ToList();

            graph[name] = refs;
        }

        return graph;
    }

    private static string ExtractProjectName(string key)
    {
        // Extract project name from key (e.g., "API" from "TaskForge.API")
        var parts = key.Split('.');
        return parts.Length > 0 ? parts.Last() : key;
    }

    private static string NormalizeAssemblyName(string assemblyName, string projectPrefix)
    {
        // Normalize assembly name for comparison
        // If it starts with project prefix, extract the layer name
        if (assemblyName.StartsWith(projectPrefix + "."))
        {
            var withoutPrefix = assemblyName.Substring(projectPrefix.Length + 1);
            var parts = withoutPrefix.Split('.');
            return parts.Length > 0 ? parts.First() : withoutPrefix;
        }
        
        var parts = assemblyName.Split('.');
        return parts.Length > 0 ? parts.First() : assemblyName;
    }
}

