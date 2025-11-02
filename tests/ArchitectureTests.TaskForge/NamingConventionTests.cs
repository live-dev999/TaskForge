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
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NetArchTest.Rules;

namespace ArchitectureTests.TaskForge;

/// <summary>
/// Tests for naming conventions consistency.
/// </summary>
public class NamingConventionTests
{
    private static readonly Assembly ApiAssembly = typeof(global::TaskForge.API.Controllers.TaskItemsController).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(global::TaskForge.Application.Core.Result<>).Assembly;
    private static readonly Assembly DomainAssembly = typeof(global::TaskForge.Domain.TaskItem).Assembly;
    private static readonly Assembly PersistenceAssembly = typeof(global::TaskForge.Persistence.DataContext).Assembly;
    private static readonly Assembly EventProcessorAssembly = typeof(global::TaskForge.EventProcessor.Controllers.EventsController).Assembly;
    private static readonly Assembly MessageConsumerAssembly = typeof(global::TaskForge.MessageConsumer.Consumers.TaskChangeEventConsumer).Assembly;

    [Fact]
    public void All_Private_Fields_Should_Start_With_Underscore()
    {
        var allAssemblies = new[] { ApiAssembly, ApplicationAssembly, DomainAssembly, 
                                   PersistenceAssembly, EventProcessorAssembly, MessageConsumerAssembly };

        var violations = new List<string>();

        foreach (var assembly in allAssemblies)
        {
            var classes = Types
                .InAssembly(assembly)
                .That()
                .AreClasses()
                .GetTypes()
                .Where(t => !t.IsNestedPrivate && t.Namespace?.StartsWith("TaskForge") == true);

            foreach (var classType in classes)
            {
                var fields = classType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .Where(f => f.IsPrivate && !f.IsLiteral && !f.Name.StartsWith("_") && !f.Name.Contains("<"));

                foreach (var field in fields)
                {
                    violations.Add($"{classType.FullName}.{field.Name}");
                }
            }
        }

        violations.Should().BeEmpty(
            $"All private fields should start with underscore. Violations:\n{string.Join("\n", violations)}");
    }

    [Fact]
    public void All_Public_Properties_Should_Use_PascalCase()
    {
        var allAssemblies = new[] { ApiAssembly, ApplicationAssembly, DomainAssembly };

        var violations = new List<string>();

        foreach (var assembly in allAssemblies)
        {
            var classes = Types
                .InAssembly(assembly)
                .That()
                .AreClasses()
                .GetTypes()
                .Where(t => t.IsPublic && t.Namespace?.StartsWith("TaskForge") == true);

            foreach (var classType in classes)
            {
                var properties = classType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(p => p.DeclaringType == classType);

                foreach (var property in properties)
                {
                    if (string.IsNullOrEmpty(property.Name) || 
                        !char.IsUpper(property.Name[0]))
                    {
                        violations.Add($"{classType.FullName}.{property.Name}");
                    }
                }
            }
        }

        violations.Should().BeEmpty(
            $"All public properties should use PascalCase. Violations:\n{string.Join("\n", violations)}");
    }

    [Fact]
    public void All_Public_Methods_Should_Use_PascalCase()
    {
        var allAssemblies = new[] { ApiAssembly, ApplicationAssembly };

        var violations = new List<string>();

        foreach (var assembly in allAssemblies)
        {
            var classes = Types
                .InAssembly(assembly)
                .That()
                .AreClasses()
                .GetTypes()
                .Where(t => t.IsPublic && t.Namespace?.StartsWith("TaskForge") == true);

            foreach (var classType in classes)
            {
                var methods = classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => !m.IsSpecialName && 
                               !m.IsConstructor &&
                               m.DeclaringType == classType);

                foreach (var method in methods)
                {
                    if (string.IsNullOrEmpty(method.Name) || 
                        !char.IsUpper(method.Name[0]))
                    {
                        violations.Add($"{classType.FullName}.{method.Name}");
                    }
                }
            }
        }

        violations.Should().BeEmpty(
            $"All public methods should use PascalCase. Violations:\n{string.Join("\n", violations)}");
    }

    [Fact]
    public void Async_Methods_Should_End_With_Async()
    {
        var allAssemblies = new[] { ApiAssembly, ApplicationAssembly, EventProcessorAssembly, MessageConsumerAssembly };

        var violations = new List<string>();

        foreach (var assembly in allAssemblies)
        {
            var classes = Types
                .InAssembly(assembly)
                .That()
                .AreClasses()
                .GetTypes()
                .Where(t => t.Namespace?.StartsWith("TaskForge") == true);

            foreach (var classType in classes)
            {
                var asyncMethods = classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(m => m.ReturnType.Name.Contains("Task") && 
                               !m.Name.EndsWith("Async") &&
                               !m.IsSpecialName);

                foreach (var method in asyncMethods)
                {
                    violations.Add($"{classType.FullName}.{method.Name}");
                }
            }
        }

        violations.Should().BeEmpty(
            $"All async methods should end with 'Async'. Violations:\n{string.Join("\n", violations)}");
    }

    [Fact]
    public void Handlers_Should_Be_Named_Handler()
    {
        var allTypes = ApplicationAssembly.GetTypes();
        var handlers = allTypes
            .Where(t => t.IsClass &&
                       !t.IsAbstract &&
                       t.GetInterfaces().Any(i => 
                           i.IsGenericType && 
                           i.GetGenericTypeDefinition().Name.StartsWith("IRequestHandler")));

        foreach (var handler in handlers)
        {
            handler.Name.Should().Be("Handler",
                $"Handler in {handler.DeclaringType?.Name ?? handler.Namespace} should be named 'Handler'");
        }
    }

    [Fact]
    public void Validators_Should_End_With_Validator()
    {
        var validators = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Validator")
            .GetTypes();

        foreach (var validator in validators)
        {
            validator.Name.Should().EndWith("Validator",
                $"Validator class {validator.Name} should end with 'Validator'");
        }
    }

    [Fact]
    public void Interfaces_Should_Start_With_I_Followed_By_Uppercase()
    {
        var allAssemblies = new[] { ApiAssembly, ApplicationAssembly, DomainAssembly, 
                                   PersistenceAssembly, EventProcessorAssembly, MessageConsumerAssembly };

        var violations = new List<string>();

        foreach (var assembly in allAssemblies)
        {
            var interfaces = Types
                .InAssembly(assembly)
                .That()
                .AreInterfaces()
                .GetTypes()
                .Where(i => i.Namespace?.StartsWith("TaskForge") == true);

            foreach (var interfaceType in interfaces)
            {
                if (!interfaceType.Name.StartsWith("I") || 
                    interfaceType.Name.Length < 2 || 
                    !char.IsUpper(interfaceType.Name[1]))
                {
                    violations.Add(interfaceType.FullName ?? interfaceType.Name);
                }
            }
        }

        violations.Should().BeEmpty(
            $"All interfaces should start with 'I' followed by uppercase letter. Violations:\n{string.Join("\n", violations)}");
    }

    [Fact]
    public void DTOs_Should_End_With_Dto()
    {
        var dtos = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Dto")
            .GetTypes();

        foreach (var dto in dtos)
        {
            dto.Name.Should().EndWith("Dto",
                $"DTO class {dto.Name} should end with 'Dto'");
        }
    }

    [Fact]
    public void Commands_Should_Be_Named_Command()
    {
        var commands = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .HaveName("Command")
            .GetTypes();

        commands.Should().NotBeEmpty(
            "Should have Command classes in Application layer");

        foreach (var command in commands)
        {
            command.Name.Should().Be("Command",
                $"Command class should be named 'Command'");
        }
    }

    [Fact]
    public void Queries_Should_Be_Named_Query()
    {
        var queries = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .HaveName("Query")
            .GetTypes();

        foreach (var query in queries)
        {
            query.Name.Should().Be("Query",
                $"Query class should be named 'Query'");
        }
    }
}

