/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

namespace ArchitectureTests.TaskForge.Core;

/// <summary>
/// Analyzes dependency graphs for architectural violations.
/// </summary>
public class DependencyGraphAnalyzer
{
    private readonly AssemblyResolver _assemblyResolver;

    public DependencyGraphAnalyzer(AssemblyResolver assemblyResolver)
    {
        _assemblyResolver = assemblyResolver ?? throw new ArgumentNullException(nameof(assemblyResolver));
    }

    /// <summary>
    /// Finds all circular dependencies in the assembly graph.
    /// </summary>
    public List<List<string>> FindCycles()
    {
        var graph = _assemblyResolver.BuildDependencyGraph();
        var visited = new HashSet<string>();
        var recStack = new HashSet<string>();
        var cycles = new List<List<string>>();

        foreach (var node in graph.Keys)
        {
            if (!visited.Contains(node))
            {
                FindCyclesInternal(node, graph, visited, recStack, new List<string>(), cycles);
            }
        }

        return cycles;
    }

    /// <summary>
    /// Validates layer dependencies according to allowed dependency rules.
    /// </summary>
    public List<string> ValidateLayerDependencies(Dictionary<string, List<string>> allowedDependencies)
    {
        var violations = new List<string>();
        var graph = _assemblyResolver.BuildDependencyGraph();

        foreach (var (assemblyName, dependencies) in graph)
        {
            if (allowedDependencies.TryGetValue(assemblyName, out var allowed))
            {
                // Normalize allowed dependencies (they might be full names or just layer names)
                var normalizedAllowed = allowed.Select(a => 
                    a.Contains('.') ? a.Split('.').Last() : a).ToList();
                
                var forbidden = dependencies.Except(normalizedAllowed).ToList();
                if (forbidden.Any())
                {
                    violations.Add($"{assemblyName} has forbidden dependencies: {string.Join(", ", forbidden)}");
                }
            }
            else
            {
                // If not in allowed dependencies, check against general rules
                // (e.g., Domain should not depend on anything)
            }
        }

        return violations;
    }

    /// <summary>
    /// Gets direct dependencies for an assembly.
    /// </summary>
    public List<string> GetDirectDependencies(string assemblyName)
    {
        var graph = _assemblyResolver.BuildDependencyGraph();
        return graph.TryGetValue(assemblyName, out var dependencies) ? dependencies : new List<string>();
    }

    private static void FindCyclesInternal(string node, Dictionary<string, List<string>> graph, 
        HashSet<string> visited, HashSet<string> recStack, List<string> path, List<List<string>> cycles)
    {
        visited.Add(node);
        recStack.Add(node);
        path.Add(node);

        if (graph.ContainsKey(node))
        {
            foreach (var neighbor in graph[node])
            {
                if (!visited.Contains(neighbor))
                {
                    FindCyclesInternal(neighbor, graph, visited, recStack, path, cycles);
                }
                else if (recStack.Contains(neighbor))
                {
                    // Found a cycle
                    var cycleStart = path.IndexOf(neighbor);
                    if (cycleStart >= 0)
                    {
                        var cycle = path.Skip(cycleStart).Concat(new[] { neighbor }).ToList();
                        cycles.Add(cycle);
                    }
                }
            }
        }

        recStack.Remove(node);
        if (path.Count > 0)
        {
            path.RemoveAt(path.Count - 1);
        }
    }
}

