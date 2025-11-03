/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using System.Reflection;
using System.Text.RegularExpressions;

namespace ArchitectureTests.TaskForge.Core;

/// <summary>
/// Analyzes source code files for architectural patterns and violations.
/// </summary>
public class SourceCodeAnalyzer
{
    private readonly AssemblyResolver _assemblyResolver;

    public SourceCodeAnalyzer(AssemblyResolver assemblyResolver)
    {
        _assemblyResolver = assemblyResolver ?? throw new ArgumentNullException(nameof(assemblyResolver));
    }

    /// <summary>
    /// Gets all source files from an assembly.
    /// </summary>
    public List<string> GetSourceFiles(Assembly assembly)
    {
        var sourceFiles = new List<string>();
        var assemblyDir = Path.GetDirectoryName(assembly.Location);
        
        if (assemblyDir == null) return sourceFiles;

        var projectDir = FindProjectDirectory(assemblyDir);
        if (projectDir == null) return sourceFiles;

        var csFiles = Directory.GetFiles(projectDir, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains("bin") && !f.Contains("obj") && !f.Contains(".g.cs"))
            .ToList();

        return csFiles;
    }

    /// <summary>
    /// Checks if a position in source code is within a comment.
    /// </summary>
    public bool IsInComment(string content, int position)
    {
        var beforePosition = content.Substring(0, position);
        
        // Check for single-line comment
        var lastNewLine = beforePosition.LastIndexOf('\n');
        var line = lastNewLine >= 0 ? beforePosition.Substring(lastNewLine) : beforePosition;
        if (line.Contains("//"))
        {
            return true;
        }
        
        // Check for multi-line comment (simplified)
        var commentStart = beforePosition.LastIndexOf("/*");
        var commentEnd = beforePosition.LastIndexOf("*/");
        if (commentStart > commentEnd)
        {
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// Gets the method context at a given position.
    /// </summary>
    public string GetMethodContext(string content, int position)
    {
        var beforePosition = content.Substring(0, position);
        var methodStart = beforePosition.LastIndexOf("public ") != -1 ? 
            beforePosition.LastIndexOf("public ") :
            (beforePosition.LastIndexOf("private ") != -1 ? beforePosition.LastIndexOf("private ") : -1);
        
        if (methodStart == -1) return "";
        
        var methodSignature = beforePosition.Substring(methodStart);
        var methodEnd = methodSignature.IndexOf('{');
        if (methodEnd > 0)
        {
            methodSignature = methodSignature.Substring(0, methodEnd);
        }
        
        return methodSignature;
    }

    /// <summary>
    /// Finds all await statements in a source file.
    /// </summary>
    public List<AwaitStatement> FindAwaitStatements(string filePath)
    {
        if (!File.Exists(filePath)) return new List<AwaitStatement>();

        var content = File.ReadAllText(filePath);
        var awaitPattern = @"await\s+([^;]+(?:\([^)]*\))?[^;]*);";
        var matches = Regex.Matches(content, awaitPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

        return matches.Cast<Match>()
            .Select(m => new AwaitStatement
            {
                Content = m.Value,
                Index = m.Index,
                LineNumber = content.Substring(0, m.Index).Split('\n').Length,
                FilePath = filePath,
                Statement = m.Groups[1].Value.Trim()
            })
            .ToList();
    }

    /// <summary>
    /// Finds all matches of a pattern in source files.
    /// </summary>
    public List<CodeMatch> FindPatternMatches(IEnumerable<string> sourceFiles, string pattern, RegexOptions options = RegexOptions.None)
    {
        var matches = new List<CodeMatch>();

        foreach (var filePath in sourceFiles)
        {
            if (!File.Exists(filePath)) continue;

            var content = File.ReadAllText(filePath);
            var regexMatches = Regex.Matches(content, pattern, options);

            foreach (Match match in regexMatches)
            {
                if (!IsInComment(content, match.Index))
                {
                    matches.Add(new CodeMatch
                    {
                        FilePath = filePath,
                        Content = match.Value,
                        Index = match.Index,
                        LineNumber = content.Substring(0, match.Index).Split('\n').Length
                    });
                }
            }
        }

        return matches;
    }

    /// <summary>
    /// Checks if a file should be excluded from analysis.
    /// </summary>
    public static bool ShouldExcludeFile(string filePath, TestConfiguration config)
    {
        var fileName = Path.GetFileName(filePath);
        var directory = Path.GetDirectoryName(filePath) ?? "";

        return filePath.Contains("Test") ||
               filePath.Contains(".g.cs") ||
               filePath.Contains("obj") ||
               filePath.Contains("bin") ||
               (config.ExcludeGeneratedFiles && fileName.Contains("Designer")) ||
               (config.ExcludeTestFiles && directory.Contains("Test"));
    }

    private static string? FindProjectDirectory(string startDir)
    {
        var dir = new DirectoryInfo(startDir);
        while (dir != null)
        {
            if (dir.GetFiles("*.csproj").Any())
            {
                return dir.FullName;
            }
            dir = dir.Parent;
        }
        return Path.GetDirectoryName(startDir);
    }
}

/// <summary>
/// Represents an await statement found in source code.
/// </summary>
public class AwaitStatement
{
    public string Content { get; set; } = string.Empty;
    public string Statement { get; set; } = string.Empty;
    public int Index { get; set; }
    public int LineNumber { get; set; }
    public string FilePath { get; set; } = string.Empty;
}

/// <summary>
/// Represents a code match found during pattern matching.
/// </summary>
public class CodeMatch
{
    public string FilePath { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Index { get; set; }
    public int LineNumber { get; set; }
}

