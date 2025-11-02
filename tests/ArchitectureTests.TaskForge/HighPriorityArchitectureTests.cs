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
 *   FITNESS FOR A PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 */

using System.Reflection;
using System.Text.RegularExpressions;
using FluentAssertions;
using NetArchTest.Rules;
using Microsoft.AspNetCore.Mvc;

namespace ArchitectureTests.TaskForge;

/// <summary>
/// High priority architecture tests for critical architectural rules.
/// These tests enforce patterns that prevent common issues like deadlocks, security vulnerabilities, and architectural violations.
/// </summary>
public class HighPriorityArchitectureTests
{
    private static readonly Assembly ApiAssembly = typeof(global::TaskForge.API.Controllers.TaskItemsController).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(global::TaskForge.Application.Core.Result<>).Assembly;
    private static readonly Assembly DomainAssembly = typeof(global::TaskForge.Domain.TaskItem).Assembly;
    private static readonly Assembly PersistenceAssembly = typeof(global::TaskForge.Persistence.DataContext).Assembly;
    private static readonly Assembly EventProcessorAssembly = typeof(global::TaskForge.EventProcessor.Controllers.EventsController).Assembly;
    private static readonly Assembly MessageConsumerAssembly = typeof(global::TaskForge.MessageConsumer.Consumers.TaskChangeEventConsumer).Assembly;

    #region Critical Test 1: DbContext in Controllers

    [Fact]
    public void Controllers_Should_Not_Use_DbContext_Directly()
    {
        var controllers = Types
            .InAssembly(ApiAssembly)
            .That()
            .Inherit(typeof(ControllerBase))
            .GetTypes();

        var violations = new List<string>();

        foreach (var controller in controllers)
        {
            // Check constructor parameters
            var constructors = controller.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            foreach (var constructor in constructors)
            {
                var hasDbContext = constructor.GetParameters()
                    .Any(p => p.ParameterType.Name == "DataContext" ||
                             p.ParameterType.Name == "DbContext" ||
                             typeof(Microsoft.EntityFrameworkCore.DbContext).IsAssignableFrom(p.ParameterType));

                if (hasDbContext)
                {
                    violations.Add($"{controller.Name} has direct dependency on DbContext");
                }
            }

            // Check fields
            var dbContextFields = controller.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .Where(f => f.FieldType.Name == "DataContext" ||
                           f.FieldType.Name == "DbContext" ||
                           typeof(Microsoft.EntityFrameworkCore.DbContext).IsAssignableFrom(f.FieldType));

            foreach (var field in dbContextFields)
            {
                violations.Add($"{controller.Name} has DbContext field: {field.Name}");
            }
        }

        violations.Should().BeEmpty(
            $"Controllers should not have direct dependencies on DbContext. Use MediatR handlers instead. Violations:\n{string.Join("\n", violations)}");
    }

    [Fact]
    public void Controllers_Should_Not_Have_Direct_Database_Operations()
    {
        var controllers = Types
            .InAssembly(ApiAssembly)
            .That()
            .Inherit(typeof(ControllerBase))
            .GetTypes();

        // This test structure is ready for source code analysis
        // Controllers should delegate to MediatR, not perform database operations directly
        foreach (var controller in controllers)
        {
            var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName);

            // Controllers should use MediatR (via BaseApiController.Mediator)
            var usesMediator = controller.BaseType?.Name == "BaseApiController" ||
                              controller.GetProperty("Mediator", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public) != null;

            usesMediator.Should().BeTrue(
                $"Controller {controller.Name} should use MediatR for database operations, not direct DbContext access");
        }
    }

    #endregion

    #region Critical Test 2: Hardcoded Values (Connection Strings, URLs, Secrets)

    [Fact]
    public void Should_Not_Have_Hardcoded_Connection_Strings()
    {
        var allAssemblies = new[] { ApiAssembly, ApplicationAssembly, PersistenceAssembly, 
                                   EventProcessorAssembly, MessageConsumerAssembly };

        var violations = new List<string>();

        foreach (var assembly in allAssemblies)
        {
            var sourceFiles = GetSourceFilesFromAssembly(assembly);
            
            foreach (var sourceFile in sourceFiles)
            {
                if (!File.Exists(sourceFile)) continue;

                var content = File.ReadAllText(sourceFile);
                
                // Check for common connection string patterns
                var connectionStringPatterns = new[]
                {
                    @"Data Source\s*=\s*[^"";]+",
                    @"Server\s*=\s*[^"";]+",
                    @"Initial Catalog\s*=\s*[^"";]+",
                    @"ConnectionString\s*=\s*""[^""]+""",
                    @"connectionString\s*[:=]\s*""[^""]+"""
                };

                foreach (var pattern in connectionStringPatterns)
                {
                    var matches = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);
                    foreach (Match match in matches)
                    {
                        // Exclude comments and configuration files
                        var lineNumber = content.Substring(0, match.Index).Split('\n').Length;
                        if (!IsInComment(content, match.Index))
                        {
                            violations.Add($"{Path.GetFileName(sourceFile)}:{lineNumber} - Potential hardcoded connection string: {match.Value.Substring(0, Math.Min(50, match.Value.Length))}...");
                        }
                    }
                }
            }
        }

        violations.Should().BeEmpty(
            $"Should not have hardcoded connection strings. Use IConfiguration instead. Violations:\n{string.Join("\n", violations.Take(10))}");
    }

    [Fact]
    public void Should_Not_Have_Hardcoded_URLs_Or_Endpoints()
    {
        var allAssemblies = new[] { ApplicationAssembly, EventProcessorAssembly, MessageConsumerAssembly };

        var violations = new List<string>();

        foreach (var assembly in allAssemblies)
        {
            var sourceFiles = GetSourceFilesFromAssembly(assembly);
            
            foreach (var sourceFile in sourceFiles)
            {
                if (!File.Exists(sourceFile)) continue;

                var content = File.ReadAllText(sourceFile);
                
                // Check for hardcoded URLs
                var urlPatterns = new[]
                {
                    @"https?://[a-zA-Z0-9.-]+(?:/[^\s""]+)?",
                    @"localhost:\d+",
                    @"127\.0\.0\.1:\d+"
                };

                foreach (var pattern in urlPatterns)
                {
                    var matches = Regex.Matches(content, pattern);
                    foreach (Match match in matches)
                    {
                        // Exclude comments and configuration/startup code
                        if (!IsInComment(content, match.Index) && 
                            !sourceFile.Contains("appsettings") &&
                            !sourceFile.Contains("launchSettings"))
                        {
                            var lineNumber = content.Substring(0, match.Index).Split('\n').Length;
                            violations.Add($"{Path.GetFileName(sourceFile)}:{lineNumber} - Potential hardcoded URL: {match.Value}");
                        }
                    }
                }
            }
        }

        // Allow some violations for localhost in development code, but warn about production concerns
        var productionViolations = violations.Where(v => !v.Contains("localhost") && !v.Contains("127.0.0.1")).ToList();
        
        productionViolations.Should().BeEmpty(
            $"Should not have hardcoded production URLs. Use IConfiguration instead. Violations:\n{string.Join("\n", productionViolations.Take(10))}");
    }

    [Fact]
    public void Should_Not_Have_Hardcoded_Secrets_Or_Passwords()
    {
        var allAssemblies = new[] { ApiAssembly, ApplicationAssembly, PersistenceAssembly, 
                                   EventProcessorAssembly, MessageConsumerAssembly };

        var violations = new List<string>();

        foreach (var assembly in allAssemblies)
        {
            var sourceFiles = GetSourceFilesFromAssembly(assembly);
            
            foreach (var sourceFile in sourceFiles)
            {
                if (!File.Exists(sourceFile)) continue;

                var content = File.ReadAllText(sourceFile);
                
                // Check for suspicious patterns that might indicate hardcoded secrets
                var secretPatterns = new[]
                {
                    @"password\s*=\s*""[^""]+""",
                    @"Password\s*=\s*""[^""]+""",
                    @"pwd\s*=\s*""[^""]+""",
                    @"api[_-]?key\s*[:=]\s*""[^""]+""",
                    @"secret\s*[:=]\s*""[^""]+""",
                    @"token\s*[:=]\s*""[^""]+""",
                    @"[A-Za-z0-9]{32,}""\s*;?\s*//.*secret|password|key" // Long strings with suspicious comments
                };

                foreach (var pattern in secretPatterns)
                {
                    var matches = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);
                    foreach (Match match in matches)
                    {
                        if (!IsInComment(content, match.Index) &&
                            !sourceFile.Contains("appsettings") &&
                            !sourceFile.Contains("test") &&
                            !match.Value.Contains("example") &&
                            !match.Value.Contains("dummy"))
                        {
                            var lineNumber = content.Substring(0, match.Index).Split('\n').Length;
                            violations.Add($"{Path.GetFileName(sourceFile)}:{lineNumber} - Potential hardcoded secret: {SanitizeSecret(match.Value)}");
                        }
                    }
                }
            }
        }

        violations.Should().BeEmpty(
            $"Should not have hardcoded secrets or passwords. Use IConfiguration, User Secrets, or environment variables. Violations:\n{string.Join("\n", violations.Take(5))}");
    }

    #endregion

    #region Critical Test 3: Circular Dependencies Between Assemblies

    [Fact]
    public void Should_Not_Have_Circular_Dependencies_Between_Assemblies()
    {
        var assemblies = new Dictionary<string, Assembly>
        {
            { "API", ApiAssembly },
            { "Application", ApplicationAssembly },
            { "Domain", DomainAssembly },
            { "Persistence", PersistenceAssembly },
            { "EventProcessor", EventProcessorAssembly },
            { "MessageConsumer", MessageConsumerAssembly }
        };

        // Build dependency graph
        var dependencyGraph = new Dictionary<string, List<string>>();
        
        foreach (var (name, assembly) in assemblies)
        {
            var refs = assembly.GetReferencedAssemblies()
                .Where(a => a.Name?.StartsWith("TaskForge") == true)
                .Select(a => a.Name!.Replace("TaskForge.", "").Split('.').First())
                .Distinct()
                .ToList();
            
            dependencyGraph[name] = refs;
        }

        // Check for cycles using DFS
        var visited = new HashSet<string>();
        var recStack = new HashSet<string>();
        var cycles = new List<List<string>>();

        foreach (var assembly in assemblies.Keys)
        {
            if (!visited.Contains(assembly))
            {
                FindCycles(assembly, dependencyGraph, visited, recStack, new List<string>(), cycles);
            }
        }

        cycles.Should().BeEmpty(
            $"Should not have circular dependencies between assemblies. Found cycles:\n{string.Join("\n", cycles.Select(c => string.Join(" -> ", c)))}");
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_Any_Other_TaskForge_Assemblies()
    {
        var domainRefs = DomainAssembly.GetReferencedAssemblies()
            .Where(a => a.Name?.StartsWith("TaskForge") == true)
            .Select(a => a.Name)
            .ToList();

        domainRefs.Should().BeEmpty(
            $"Domain layer should not depend on any other TaskForge assemblies. Found dependencies: {string.Join(", ", domainRefs)}");
    }

    [Fact]
    public void Application_Should_Only_Depend_On_Domain()
    {
        var applicationRefs = ApplicationAssembly.GetReferencedAssemblies()
            .Where(a => a.Name?.StartsWith("TaskForge") == true)
            .Select(a => a.Name)
            .ToList();

        // Application can depend on Domain and potentially Persistence (if using EF Core directly)
        // But should not depend on API, EventProcessor, MessageConsumer
        var forbiddenDeps = applicationRefs
            .Where(r => r != "TaskForge.Domain" && 
                       r != "TaskForge.Persistence") // Allowed for EF Core DbContext
            .ToList();

        forbiddenDeps.Should().BeEmpty(
            $"Application layer should only depend on Domain (and optionally Persistence for EF Core). Found forbidden dependencies: {string.Join(", ", forbiddenDeps)}");
    }

    #endregion

    #region Critical Test 4: Blocking Calls in Async Methods

    [Fact]
    public void Async_Methods_Should_Not_Use_Blocking_Calls()
    {
        var allAssemblies = new[] { ApiAssembly, ApplicationAssembly, EventProcessorAssembly, MessageConsumerAssembly };

        var violations = new List<string>();

        foreach (var assembly in allAssemblies)
        {
            var sourceFiles = GetSourceFilesFromAssembly(assembly);
            
            foreach (var sourceFile in sourceFiles)
            {
                if (!File.Exists(sourceFile)) continue;

                var content = File.ReadAllText(sourceFile);
                
                // Look for async methods
                var asyncMethodPattern = @"async\s+(Task|Task<|void)\s+\w+\s*\([^)]*\)\s*\{[^}]*\}";
                var asyncMethods = Regex.Matches(content, asyncMethodPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

                // Check each async method for blocking calls
                foreach (Match asyncMethod in asyncMethods)
                {
                    var methodBody = asyncMethod.Value;
                    
                    // Check for blocking patterns
                    var blockingPatterns = new[]
                    {
                        @"\.Wait\(\)",
                        @"\.Result\b",
                        @"GetAwaiter\(\)\.GetResult\(\)",
                        @"\.WaitAsync\(\)", // Sometimes misused
                    };

                    foreach (var pattern in blockingPatterns)
                    {
                        if (Regex.IsMatch(methodBody, pattern))
                        {
                            var lineNumber = content.Substring(0, asyncMethod.Index).Split('\n').Length;
                            violations.Add($"{Path.GetFileName(sourceFile)}:{lineNumber} - Async method contains blocking call: {pattern}");
                        }
                    }
                }
            }
        }

        violations.Should().BeEmpty(
            $"Async methods should not use blocking calls (.Wait(), .Result, GetAwaiter().GetResult()). Use await instead. Violations:\n{string.Join("\n", violations.Take(10))}");
    }

    #endregion

    #region Critical Test 5: ConfigureAwait(false) in Library Code

    [Fact]
    public void Library_Code_Should_Use_ConfigureAwait_False()
    {
        // Library code (Application, Persistence layers) should use ConfigureAwait(false)
        // to prevent deadlocks and improve performance when called from UI context
        var libraryAssemblies = new[] { ApplicationAssembly, PersistenceAssembly };
        
        var violations = new List<string>();

        foreach (var assembly in libraryAssemblies)
        {
            var sourceFiles = GetSourceFilesFromAssembly(assembly);
            
            foreach (var sourceFile in sourceFiles)
            {
                if (!File.Exists(sourceFile)) continue;

                var content = File.ReadAllText(sourceFile);
                
                // Skip test files and generated files
                if (sourceFile.Contains("Test") || 
                    sourceFile.Contains(".g.cs") || 
                    sourceFile.Contains("obj") || 
                    sourceFile.Contains("bin"))
                {
                    continue;
                }

                // Find all await statements - improved pattern to capture multiline
                var awaitPattern = @"await\s+([^;]+(?:\([^)]*\))?[^;]*);";
                var awaitStatements = Regex.Matches(content, awaitPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

                foreach (Match awaitStatement in awaitStatements)
                {
                    var statement = awaitStatement.Value;
                    var lineNumber = content.Substring(0, awaitStatement.Index).Split('\n').Length;
                    
                    // Skip if ConfigureAwait is already present
                    if (statement.Contains("ConfigureAwait"))
                    {
                        // Verify it's ConfigureAwait(false), not ConfigureAwait(true)
                        if (statement.Contains("ConfigureAwait(true)"))
                        {
                            violations.Add($"{Path.GetFileName(sourceFile)}:{lineNumber} - Should use ConfigureAwait(false), not ConfigureAwait(true)");
                        }
                        continue;
                    }
                    
                    // Exclude cases where we might need synchronization context
                    var needsContext = statement.Contains("HttpContext") ||
                                      statement.Contains("SynchronizationContext") ||
                                      statement.Contains("Dispatcher") ||
                                      statement.Contains("CaptureExecutionContext") ||
                                      sourceFile.Contains("Program.cs") ||
                                      sourceFile.Contains("Startup.cs");
                    
                    if (needsContext)
                    {
                        continue;
                    }
                    
                    // Check method context - skip if in UI/API context requiring synchronization
                    var methodContext = GetMethodContext(content, awaitStatement.Index);
                    var isInContextRequiringSync = methodContext.Contains("Controller") || 
                                                  methodContext.Contains("Middleware") ||
                                                  methodContext.Contains("Program") ||
                                                  methodContext.Contains("Startup") ||
                                                  methodContext.Contains("Page") ||
                                                  methodContext.Contains("View");
                    
                    if (isInContextRequiringSync)
                    {
                        continue;
                    }
                    
                    // Check for common async operations that should use ConfigureAwait(false)
                    var asyncOperations = new[]
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
                    
                    var isLibraryAsyncOperation = asyncOperations.Any(op => statement.Contains(op));
                    
                    if (isLibraryAsyncOperation)
                    {
                        violations.Add($"{Path.GetFileName(sourceFile)}:{lineNumber} - Library code should use ConfigureAwait(false) for: {statement.Trim()}");
                    }
                }
            }
        }

        violations.Should().BeEmpty(
            $"Library code should use ConfigureAwait(false) to prevent deadlocks and improve performance. Violations:\n{string.Join("\n", violations.Take(15))}");
    }

    [Fact]
    public void Application_Layer_Handlers_Should_Use_ConfigureAwait_False()
    {
        // Specifically check handlers in Application layer
        var violations = new List<string>();
        var sourceFiles = GetSourceFilesFromAssembly(ApplicationAssembly);
        
        foreach (var sourceFile in sourceFiles)
        {
            if (!File.Exists(sourceFile)) continue;
            if (!sourceFile.Contains("TaskItems")) continue; // Focus on handlers
            
            var content = File.ReadAllText(sourceFile);
            
            // Find await statements in handlers
            var awaitPattern = @"await\s+([^;]+(?:\([^)]*\))?[^;]*);";
            var awaitStatements = Regex.Matches(content, awaitPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            
            foreach (Match awaitStatement in awaitStatements)
            {
                var statement = awaitStatement.Value;
                var lineNumber = content.Substring(0, awaitStatement.Index).Split('\n').Length;
                
                // Skip if ConfigureAwait is already present
                if (statement.Contains("ConfigureAwait"))
                {
                    continue;
                }
                
                // Handlers are library code - should use ConfigureAwait(false)
                var isDbContextOperation = statement.Contains("SaveChangesAsync") ||
                                          statement.Contains("FindAsync") ||
                                          statement.Contains("ToListAsync") ||
                                          statement.Contains("AddAsync") ||
                                          statement.Contains("Remove");
                
                var isServiceCall = statement.Contains("SendEventAsync") ||
                                   statement.Contains("PublishEventAsync") ||
                                   statement.Contains("PostAsJsonAsync");
                
                if (isDbContextOperation || isServiceCall)
                {
                    violations.Add($"{Path.GetFileName(sourceFile)}:{lineNumber} - Handler should use ConfigureAwait(false) for: {statement.Trim()}");
                }
            }
        }
        
        violations.Should().BeEmpty(
            $"Application layer handlers should use ConfigureAwait(false) for async operations. Violations:\n{string.Join("\n", violations.Take(10))}");
    }

    [Fact]
    public void EventService_Should_Use_ConfigureAwait_False()
    {
        // EventService is a library service - should use ConfigureAwait(false)
        var sourceFiles = GetSourceFilesFromAssembly(ApplicationAssembly);
        var eventServiceFile = sourceFiles.FirstOrDefault(f => f.Contains("EventService.cs"));
        
        if (eventServiceFile == null || !File.Exists(eventServiceFile))
        {
            return; // EventService might not exist
        }
        
        var violations = new List<string>();
        var content = File.ReadAllText(eventServiceFile);
        
        var awaitPattern = @"await\s+([^;]+(?:\([^)]*\))?[^;]*);";
        var awaitStatements = Regex.Matches(content, awaitPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        
        foreach (Match awaitStatement in awaitStatements)
        {
            var statement = awaitStatement.Value;
            var lineNumber = content.Substring(0, awaitStatement.Index).Split('\n').Length;
            
            if (statement.Contains("ConfigureAwait"))
            {
                if (statement.Contains("ConfigureAwait(true)"))
                {
                    violations.Add($"{Path.GetFileName(eventServiceFile)}:{lineNumber} - Should use ConfigureAwait(false), not ConfigureAwait(true)");
                }
                continue;
            }
            
            // EventService makes HTTP calls - should use ConfigureAwait(false)
            if (statement.Contains("PostAsJsonAsync") || 
                statement.Contains("ReadAsStringAsync") ||
                statement.Contains("HttpClient"))
            {
                violations.Add($"{Path.GetFileName(eventServiceFile)}:{lineNumber} - EventService should use ConfigureAwait(false) for HTTP operations: {statement.Trim()}");
            }
        }
        
        violations.Should().BeEmpty(
            $"EventService should use ConfigureAwait(false) for all async operations. Violations:\n{string.Join("\n", violations)}");
    }

    [Fact]
    public void Should_Not_Use_ConfigureAwait_True_In_Library_Code()
    {
        // Library code should never use ConfigureAwait(true) - it's the default and explicit usage suggests misunderstanding
        var libraryAssemblies = new[] { ApplicationAssembly, PersistenceAssembly };
        var violations = new List<string>();
        
        foreach (var assembly in libraryAssemblies)
        {
            var sourceFiles = GetSourceFilesFromAssembly(assembly);
            
            foreach (var sourceFile in sourceFiles)
            {
                if (!File.Exists(sourceFile)) continue;
                if (sourceFile.Contains("Test") || sourceFile.Contains(".g.cs")) continue;
                
                var content = File.ReadAllText(sourceFile);
                
                // Find ConfigureAwait(true) usage
                var configureAwaitTruePattern = @"ConfigureAwait\s*\(\s*true\s*\)";
                var matches = Regex.Matches(content, configureAwaitTruePattern, RegexOptions.IgnoreCase);
                
                foreach (Match match in matches)
                {
                    var lineNumber = content.Substring(0, match.Index).Split('\n').Length;
                    violations.Add($"{Path.GetFileName(sourceFile)}:{lineNumber} - Should not use ConfigureAwait(true) in library code. Use ConfigureAwait(false) or omit it (default is true)");
                }
            }
        }
        
        violations.Should().BeEmpty(
            $"Library code should not explicitly use ConfigureAwait(true). Violations:\n{string.Join("\n", violations)}");
    }

    [Fact]
    public void DbContext_Operations_Should_Use_ConfigureAwait_False()
    {
        // DbContext operations in Application/Persistence layers should use ConfigureAwait(false)
        var libraryAssemblies = new[] { ApplicationAssembly, PersistenceAssembly };
        var violations = new List<string>();
        
        var dbContextOperations = new[]
        {
            "SaveChangesAsync",
            "FindAsync",
            "ToListAsync",
            "FirstOrDefaultAsync",
            "SingleOrDefaultAsync",
            "AnyAsync",
            "CountAsync",
            "AddAsync",
            "AddRangeAsync",
            "Remove",
            "Update",
            "Entry"
        };
        
        foreach (var assembly in libraryAssemblies)
        {
            var sourceFiles = GetSourceFilesFromAssembly(assembly);
            
            foreach (var sourceFile in sourceFiles)
            {
                if (!File.Exists(sourceFile)) continue;
                if (sourceFile.Contains("Test") || sourceFile.Contains(".g.cs")) continue;
                
                var content = File.ReadAllText(sourceFile);
                
                // Find await statements with DbContext operations
                foreach (var operation in dbContextOperations)
                {
                    var pattern = $@"await\s+[^;]*{operation}[^;]*;";
                    var matches = Regex.Matches(content, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    
                    foreach (Match match in matches)
                    {
                        if (match.Value.Contains("ConfigureAwait"))
                        {
                            if (match.Value.Contains("ConfigureAwait(true)"))
                            {
                                var lineNumber = content.Substring(0, match.Index).Split('\n').Length;
                                violations.Add($"{Path.GetFileName(sourceFile)}:{lineNumber} - DbContext operation should use ConfigureAwait(false), not ConfigureAwait(true)");
                            }
                        }
                        else
                        {
                            var lineNumber = content.Substring(0, match.Index).Split('\n').Length;
                            violations.Add($"{Path.GetFileName(sourceFile)}:{lineNumber} - DbContext operation {operation} should use ConfigureAwait(false)");
                        }
                    }
                }
            }
        }
        
        violations.Should().BeEmpty(
            $"DbContext operations in library code should use ConfigureAwait(false). Violations:\n{string.Join("\n", violations.Take(10))}");
    }

    #endregion

    #region Helper Methods

    private static List<string> GetSourceFilesFromAssembly(Assembly assembly)
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

    private static bool IsInComment(string content, int position)
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

    private static string SanitizeSecret(string secret)
    {
        // Replace middle part with asterisks
        if (secret.Length > 20)
        {
            return secret.Substring(0, 5) + "..." + secret.Substring(secret.Length - 5);
        }
        return "***";
    }

    private static string GetMethodContext(string content, int position)
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

    private static void FindCycles(string node, Dictionary<string, List<string>> graph, 
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
                    FindCycles(neighbor, graph, visited, recStack, path, cycles);
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
        path.RemoveAt(path.Count - 1);
    }

    #endregion
}

