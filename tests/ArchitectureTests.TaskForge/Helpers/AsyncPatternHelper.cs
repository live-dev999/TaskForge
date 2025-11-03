/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using System.Text.RegularExpressions;
using ArchitectureTests.TaskForge.Core;

namespace ArchitectureTests.TaskForge.Helpers;

/// <summary>
/// Helper methods for checking async/await patterns.
/// </summary>
public static class AsyncPatternHelper
{
    /// <summary>
    /// Checks if await statements use ConfigureAwait(false) where required.
    /// </summary>
    public static List<Violation> FindMissingConfigureAwait(
        SourceCodeAnalyzer analyzer,
        IEnumerable<string> sourceFiles,
        TestConfiguration config)
    {
        var violations = new List<Violation>();

        foreach (var sourceFile in sourceFiles)
        {
            if (SourceCodeAnalyzer.ShouldExcludeFile(sourceFile, config)) continue;

            var awaitStatements = analyzer.FindAwaitStatements(sourceFile);

            foreach (var awaitStmt in awaitStatements)
            {
                // Skip if ConfigureAwait is already present
                if (awaitStmt.Content.Contains("ConfigureAwait"))
                {
                    if (awaitStmt.Content.Contains("ConfigureAwait(true)"))
                    {
                        violations.Add(new Violation
                        {
                            FilePath = sourceFile,
                            LineNumber = awaitStmt.LineNumber,
                            Message = "Should use ConfigureAwait(false), not ConfigureAwait(true)",
                            Code = awaitStmt.Content
                        });
                    }
                    continue;
                }

                // Check if context is required
                var needsContext = config.ContextRequiringNamespaces.Any(ctx => 
                    awaitStmt.Content.Contains(ctx) || 
                    sourceFile.Contains(ctx + ".cs"));

                if (needsContext) continue;

                // Check if it's a library async operation
                var isLibraryOperation = config.AsyncOperationsRequiringConfigureAwait.Any(op =>
                    awaitStmt.Statement.Contains(op));

                if (isLibraryOperation)
                {
                    violations.Add(new Violation
                    {
                        FilePath = sourceFile,
                        LineNumber = awaitStmt.LineNumber,
                        Message = $"Library code should use ConfigureAwait(false) for: {awaitStmt.Statement}",
                        Code = awaitStmt.Content
                    });
                }
            }
        }

        return violations;
    }

    /// <summary>
    /// Finds blocking calls in async methods.
    /// </summary>
    public static List<Violation> FindBlockingCalls(
        SourceCodeAnalyzer analyzer,
        IEnumerable<string> sourceFiles,
        TestConfiguration config)
    {
        var violations = new List<Violation>();

        foreach (var pattern in config.BlockingCallPatterns)
        {
            var matches = analyzer.FindPatternMatches(sourceFiles, pattern, RegexOptions.IgnoreCase);
            
            foreach (var match in matches)
            {
                violations.Add(new Violation
                {
                    FilePath = match.FilePath,
                    LineNumber = match.LineNumber,
                    Message = $"Async method contains blocking call: {pattern}",
                    Code = match.Content
                });
            }
        }

        return violations;
    }
}

/// <summary>
/// Represents an architectural violation found in code.
/// </summary>
public class Violation
{
    public string FilePath { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public override string ToString()
    {
        var fileName = Path.GetFileName(FilePath);
        return $"{fileName}:{LineNumber} - {Message}\n  Code: {Code.Trim()}";
    }
}

