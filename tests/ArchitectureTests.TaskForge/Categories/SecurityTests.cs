/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;
using FluentAssertions;
using ArchitectureTests.TaskForge.Core;

namespace ArchitectureTests.TaskForge.Categories;

/// <summary>
/// Tests for security-related architectural rules.
/// </summary>
public class SecurityTests : ArchitectureTestBase
{
    protected override Dictionary<string, Assembly> GetAssemblies()
    {
        return new Dictionary<string, Assembly>
        {
            { "API", typeof(global::TaskForge.API.Controllers.TaskItemsController).Assembly },
            { "Application", typeof(global::TaskForge.Application.Core.Result<>).Assembly },
            { "Persistence", typeof(global::TaskForge.Persistence.DataContext).Assembly }
        };
    }

    [Fact]
    public void Should_Not_Have_Hardcoded_Connection_Strings()
    {
        var violations = new List<string>();
        var assemblies = GetProductionAssemblies();

        var connectionStringPatterns = new[]
        {
            @"Data Source\s*=\s*[^"";]+",
            @"Server\s*=\s*[^"";]+",
            @"Initial Catalog\s*=\s*[^"";]+",
            @"ConnectionString\s*=\s*""[^""]+"""
        };

        foreach (var assembly in assemblies)
        {
            var sourceFiles = GetSourceFiles(assembly);
            
            foreach (var sourceFile in sourceFiles)
            {
                if (!File.Exists(sourceFile)) continue;
                if (sourceFile.Contains("appsettings") || sourceFile.Contains("launchSettings")) continue;

                var content = File.ReadAllText(sourceFile);

                foreach (var pattern in connectionStringPatterns)
                {
                    var matches = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);
                    foreach (Match match in matches)
                    {
                        if (!SourceAnalyzer.IsInComment(content, match.Index))
                        {
                            var lineNumber = content.Substring(0, match.Index).Split('\n').Length;
                            violations.Add($"{Path.GetFileName(sourceFile)}:{lineNumber} - Potential hardcoded connection string");
                        }
                    }
                }
            }
        }

        violations.Should().BeEmpty(
            $"Should not have hardcoded connection strings. Use IConfiguration instead. Violations:\n{string.Join("\n", violations.Take(10))}");
    }

    [Fact]
    public void Should_Not_Have_Hardcoded_Secrets()
    {
        var violations = new List<string>();
        var assemblies = GetProductionAssemblies();

        var secretPatterns = new[]
        {
            @"password\s*=\s*""[^""]+""",
            @"api[_-]?key\s*[:=]\s*""[^""]+""",
            @"secret\s*[:=]\s*""[^""]+"""
        };

        foreach (var assembly in assemblies)
        {
            var sourceFiles = GetSourceFiles(assembly);
            
            foreach (var sourceFile in sourceFiles)
            {
                if (!File.Exists(sourceFile)) continue;
                if (sourceFile.Contains("appsettings") || sourceFile.Contains("test")) continue;

                var content = File.ReadAllText(sourceFile);

                foreach (var pattern in secretPatterns)
                {
                    var matches = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);
                    foreach (Match match in matches)
                    {
                        if (!SourceAnalyzer.IsInComment(content, match.Index) &&
                            !match.Value.Contains("example") &&
                            !match.Value.Contains("dummy"))
                        {
                            var lineNumber = content.Substring(0, match.Index).Split('\n').Length;
                            violations.Add($"{Path.GetFileName(sourceFile)}:{lineNumber} - Potential hardcoded secret");
                        }
                    }
                }
            }
        }

        violations.Should().BeEmpty(
            $"Should not have hardcoded secrets. Use IConfiguration, User Secrets, or environment variables. Violations:\n{string.Join("\n", violations.Take(5))}");
    }
}

