/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

namespace Tests.TaskForge.Architecture.Core;

/// <summary>
/// Configuration for architecture tests.
/// </summary>
public class TestConfiguration
{
    /// <summary>
    /// Gets or sets whether to exclude generated files.
    /// </summary>
    public bool ExcludeGeneratedFiles { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to exclude test files.
    /// </summary>
    public bool ExcludeTestFiles { get; set; } = true;

    /// <summary>
    /// Gets or sets the project name prefix (e.g., "TaskForge").
    /// </summary>
    public string ProjectPrefix { get; set; } = "TaskForge";

    /// <summary>
    /// Gets the full assembly name for a given layer.
    /// </summary>
    public string GetAssemblyName(string layerName)
    {
        return $"{ProjectPrefix}.{layerName}";
    }

    /// <summary>
    /// Gets all layer names used in the project.
    /// </summary>
    public List<string> LayerNames { get; set; } = new()
    {
        "API",
        "Application",
        "Domain",
        "Persistence",
        "EventProcessor",
        "MessageConsumer"
    };

    /// <summary>
    /// Gets or sets patterns to exclude from source code analysis.
    /// </summary>
    public List<string> ExcludePatterns { get; set; } = new()
    {
        "bin",
        "obj",
        ".g.cs",
        "Test",
        "Designer"
    };

    /// <summary>
    /// Gets or sets namespaces that require synchronization context (should not use ConfigureAwait(false)).
    /// </summary>
    public List<string> ContextRequiringNamespaces { get; set; } = new()
    {
        "HttpContext",
        "SynchronizationContext",
        "Dispatcher",
        "Program",
        "Startup"
    };

    /// <summary>
    /// Gets or sets common async operations that should use ConfigureAwait(false).
    /// </summary>
    public List<string> AsyncOperationsRequiringConfigureAwait { get; set; } = new()
    {
        "SaveChangesAsync",
        "FindAsync",
        "ToListAsync",
        "PostAsJsonAsync",
        "ReadAsStringAsync",
        "PublishAsync",
        "SendAsync",
        "GetAsync",
        "PutAsync",
        "DeleteAsync"
    };

    /// <summary>
    /// Gets or sets blocking call patterns to detect.
    /// </summary>
    public List<string> BlockingCallPatterns { get; set; } = new()
    {
        @"\.Wait\(\)",
        @"\.Result\b",
        @"GetAwaiter\(\)\.GetResult\(\)"
    };
}

