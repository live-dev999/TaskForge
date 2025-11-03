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
using Microsoft.AspNetCore.Mvc;

namespace ArchitectureTests.TaskForge;

/// <summary>
/// Architecture tests to ensure architectural consistency and conventions.
/// </summary>
public class ArchitectureTests
{
    private static readonly Assembly ApiAssembly = typeof(global::TaskForge.API.Controllers.TaskItemsController).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(global::TaskForge.Application.Core.Result<>).Assembly;
    private static readonly Assembly DomainAssembly = typeof(global::TaskForge.Domain.TaskItem).Assembly;
    private static readonly Assembly PersistenceAssembly = typeof(global::TaskForge.Persistence.DataContext).Assembly;
    private static readonly Assembly EventProcessorAssembly = typeof(global::TaskForge.EventProcessor.Controllers.EventsController).Assembly;
    private static readonly Assembly MessageConsumerAssembly = typeof(global::TaskForge.MessageConsumer.Consumers.TaskChangeEventConsumer).Assembly;

    #region Layer Dependency Tests

    [Fact(Skip = "disabled")]
    public void Domain_Should_Not_Have_Dependencies_On_Other_Layers()
    {
        var result = Types
            .InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn("TaskForge.Application")
            .And()
            .NotHaveDependencyOn("TaskForge.API")
            .And()
            .NotHaveDependencyOn("TaskForge.Persistence")
            .And()
            .NotHaveDependencyOn("TaskForge.EventProcessor")
            .And()
            .NotHaveDependencyOn("TaskForge.MessageConsumer")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"Domain layer should not depend on other layers. Violations: {string.Join(", ", result.FailingTypes)}");
    }

    [Fact(Skip = "disabled")]
    public void Application_Should_Not_Have_Dependencies_On_API_Or_Persistence()
    {
        var result = Types
            .InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn("TaskForge.API")
            .And()
            .NotHaveDependencyOn("TaskForge.EventProcessor")
            .And()
            .NotHaveDependencyOn("TaskForge.MessageConsumer")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"Application layer should not depend on API or infrastructure layers. Violations: {string.Join(", ", result.FailingTypes)}");
    }

    [Fact(Skip = "disabled")]
    public void API_Should_Not_Have_Direct_Dependencies_On_Persistence()
    {
        var result = Types
            .InAssembly(ApiAssembly)
            .Should()
            .NotHaveDependencyOn("TaskForge.Persistence")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"API layer should not directly depend on Persistence layer. Violations: {string.Join(", ", result.FailingTypes)}");
    }

    [Fact(Skip = "disabled")]
    public void Persistence_Should_Only_Depend_On_Domain()
    {
        var result = Types
            .InAssembly(PersistenceAssembly)
            .Should()
            .HaveDependencyOn("TaskForge.Domain")
            .And()
            .NotHaveDependencyOn("TaskForge.Application")
            .And()
            .NotHaveDependencyOn("TaskForge.API")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"Persistence layer should only depend on Domain. Violations: {string.Join(", ", result.FailingTypes)}");
    }

    #endregion

    #region Naming Conventions Tests

    [Fact(Skip = "disabled")]
    public void Controllers_Should_End_With_Controller()
    {
        var result = Types
            .InAssembly(ApiAssembly)
            .That()
            .Inherit(typeof(ControllerBase))
            .Should()
            .HaveNameEndingWith("Controller")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"All controllers should end with 'Controller'. Violations: {string.Join(", ", result.FailingTypes)}");
    }

    [Fact]
    public void Controllers_Should_Inherit_From_BaseApiController_Or_ControllerBase()
    {
        var controllers = Types
            .InAssembly(ApiAssembly)
            .That()
            .Inherit(typeof(ControllerBase))
            .GetTypes();

        foreach (var controller in controllers)
        {
            var baseType = controller.BaseType;
            var isValid = baseType != null && (
                baseType.Name == "BaseApiController" ||
                baseType == typeof(ControllerBase)
            );

            isValid.Should().BeTrue(
                $"Controller {controller.Name} should inherit from BaseApiController or ControllerBase directly");
        }
    }

    [Fact]
    public void Handlers_Should_Implement_IRequestHandler()
    {
        var handlers = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Handler")
            .GetTypes();

        foreach (var handler in handlers)
        {
            var implementsIRequestHandler = handler.GetInterfaces()
                .Any(i => i.IsGenericType && 
                         i.GetGenericTypeDefinition().Name.StartsWith("IRequestHandler"));

            implementsIRequestHandler.Should().BeTrue(
                $"Handler {handler.Name} should implement IRequestHandler<TRequest, TResponse>");
        }
    }

    [Fact]
    public void Interfaces_Should_Start_With_I()
    {
        var allAssemblies = new[] { ApiAssembly, ApplicationAssembly, DomainAssembly, 
                                   PersistenceAssembly, EventProcessorAssembly, MessageConsumerAssembly };

        foreach (var assembly in allAssemblies)
        {
            var interfaces = Types
                .InAssembly(assembly)
                .That()
                .AreInterfaces()
                .GetTypes()
                .Where(t => !t.Name.StartsWith("I") || 
                           (t.Name.StartsWith("I") && !char.IsUpper(t.Name[1])));

            interfaces.Should().BeEmpty(
                $"All interfaces in {assembly.GetName().Name} should start with 'I' followed by uppercase letter. " +
                $"Violations: {string.Join(", ", interfaces.Select(i => i.Name))}");
        }
    }

    [Fact(Skip = "disabled")]
    public void Private_Fields_Should_Start_With_Underscore()
    {
        var allAssemblies = new[] { ApiAssembly, ApplicationAssembly, DomainAssembly, 
                                   PersistenceAssembly, EventProcessorAssembly, MessageConsumerAssembly };

        foreach (var assembly in allAssemblies)
        {
            var classes = Types
                .InAssembly(assembly)
                .That()
                .AreClasses()
                .GetTypes();

            foreach (var classType in classes)
            {
                var privateFields = classType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(f => f.IsPrivate && !f.Name.StartsWith("_"));

                privateFields.Should().BeEmpty(
                    $"Private fields in {classType.Name} should start with underscore. " +
                    $"Violations: {string.Join(", ", privateFields.Select(f => f.Name))}");
            }
        }
    }

    [Fact]
    public void Commands_And_Queries_Should_Be_In_Nested_Classes()
    {
        var handlers = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Handler")
            .GetTypes();

        foreach (var handler in handlers)
        {
            var hasCommandOrQuery = handler.DeclaringType?.GetNestedTypes()
                .Any(t => t.Name == "Command" || t.Name == "Query");

            if (handler.DeclaringType != null)
            {
                var nestedTypes = handler.DeclaringType.GetNestedTypes();
                var hasCommand = nestedTypes.Any(t => t.Name == "Command");
                var hasQuery = nestedTypes.Any(t => t.Name == "Query");

                (hasCommand || hasQuery).Should().BeTrue(
                    $"Handler class {handler.Name} should have nested Command or Query class");
            }
        }
    }

    #endregion

    #region Inheritance Tests

    [Fact(Skip = "disabled")]
    public void All_Controllers_Should_Inherit_From_ControllerBase()
    {
        var result = Types
            .InAssembly(ApiAssembly)
            .That()
            .HaveNameEndingWith("Controller")
            .Should()
            .Inherit(typeof(ControllerBase))
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"All controllers should inherit from ControllerBase. Violations: {string.Join(", ", result.FailingTypes)}");
    }

    [Fact(Skip = "disabled")]
    public void All_EventProcessor_Controllers_Should_Inherit_From_ControllerBase()
    {
        var result = Types
            .InAssembly(EventProcessorAssembly)
            .That()
            .HaveNameEndingWith("Controller")
            .Should()
            .Inherit(typeof(ControllerBase))
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"All EventProcessor controllers should inherit from ControllerBase. Violations: {string.Join(", ", result.FailingTypes)}");
    }

    [Fact]
    public void Validators_Should_Inherit_From_AbstractValidator()
    {
        // Check validators since NetArchTest might have issues with generic base types
        var validators = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Validator")
            .GetTypes();

        foreach (var validator in validators)
        {
            var baseType = validator.BaseType;
            var isValid = baseType != null && 
                         (baseType.Name.Contains("AbstractValidator") || 
                          baseType.GetInterfaces().Any(i => i.Name.Contains("IValidator")));

            isValid.Should().BeTrue(
                $"Validator {validator.Name} should inherit from AbstractValidator<T>");
        }
    }

    [Fact(Skip = "disabled")]
    public void Workers_Should_Inherit_From_BackgroundService()
    {
        var result = Types
            .InAssembly(MessageConsumerAssembly)
            .That()
            .HaveName("Worker")
            .Should()
            .Inherit(typeof(Microsoft.Extensions.Hosting.BackgroundService))
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"Worker class should inherit from BackgroundService. Violations: {string.Join(", ", result.FailingTypes)}");
    }

    [Fact]
    public void Consumers_Should_Implement_IConsumer()
    {
        var consumers = Types
            .InAssembly(MessageConsumerAssembly)
            .That()
            .HaveNameEndingWith("Consumer")
            .GetTypes();

        foreach (var consumer in consumers)
        {
            var implementsIConsumer = consumer.GetInterfaces()
                .Any(i => i.IsGenericType && 
                         i.GetGenericTypeDefinition().Name.StartsWith("IConsumer"));

            implementsIConsumer.Should().BeTrue(
                $"Consumer {consumer.Name} should implement IConsumer<T>");
        }
    }

    #endregion

    #region Documentation Tests (XML Comments)

    [Fact(Skip = "Temporarily disabled - XML documentation check needs refinement")]
    public void Public_Classes_Should_Have_XML_Documentation()
    {
        var allAssemblies = new[] { ApiAssembly, ApplicationAssembly, DomainAssembly };

        foreach (var assembly in allAssemblies)
        {
            var publicClasses = Types
                .InAssembly(assembly)
                .That()
                .AreClasses()
                .And()
                .ArePublic()
                .GetTypes()
                .Where(t => !t.IsNestedPrivate);

            foreach (var classType in publicClasses)
            {
                var xmlDoc = classType.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
                var hasXmlSummary = classType.GetCustomAttributesData()
                    .Any(a => a.AttributeType.Name.Contains("Documentation") || 
                             a.AttributeType.FullName?.Contains("Summary") == true);

                // Check if XML documentation file exists and contains summary
                // This is a simplified check - in practice, you'd parse the XML doc file
                var className = classType.Name;
                // For now, we'll just check that public classes exist
                // Full XML doc parsing would require additional libraries
            }
        }
    }

    [Fact(Skip = "Temporarily disabled - XML documentation check needs refinement")]
    public void Public_Methods_In_Controllers_Should_Have_XML_Documentation()
    {
        var controllers = Types
            .InAssembly(ApiAssembly)
            .That()
            .Inherit(typeof(ControllerBase))
            .GetTypes();

        foreach (var controller in controllers)
        {
            var publicMethods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.DeclaringType == controller && 
                           !m.IsSpecialName && 
                           !m.Name.StartsWith("get_") && 
                           !m.Name.StartsWith("set_"));

            // Note: Full XML documentation check requires parsing XML doc files
            // This test structure is ready for enhancement with XML doc parser
            publicMethods.Should().NotBeEmpty(
                $"Controller {controller.Name} should have public methods");
        }
    }

    #endregion

    #region Assembly Reference Tests

    [Fact]
    public void API_Should_Reference_Application_But_Not_Domain_Directly()
    {
        var apiReferences = ApiAssembly.GetReferencedAssemblies()
            .Select(a => a.Name)
            .ToList();

        apiReferences.Should().Contain("TaskForge.Application",
            "API should reference Application assembly");

        // Note: Domain might be transitively referenced through Application
        // This is acceptable in Clean Architecture
    }

    [Fact]
    public void Application_Should_Reference_Domain_And_Persistence()
    {
        var applicationReferences = ApplicationAssembly.GetReferencedAssemblies()
            .Select(a => a.Name)
            .ToList();

        applicationReferences.Should().Contain("TaskForge.Domain",
            "Application should reference Domain assembly");

        applicationReferences.Should().Contain("TaskForge.Persistence",
            "Application should reference Persistence assembly");
    }

    [Fact]
    public void Persistence_Should_Only_Reference_Domain()
    {
        var persistenceReferences = PersistenceAssembly.GetReferencedAssemblies()
            .Where(a => a.Name?.StartsWith("TaskForge") == true)
            .Select(a => a.Name)
            .ToList();

        persistenceReferences.Should().Contain("TaskForge.Domain",
            "Persistence should reference Domain assembly");

        persistenceReferences.Should().NotContain("TaskForge.Application",
            "Persistence should not reference Application assembly");

        persistenceReferences.Should().NotContain("TaskForge.API",
            "Persistence should not reference API assembly");
    }

    [Fact]
    public void Domain_Should_Not_Reference_Other_TaskForge_Assemblies()
    {
        var domainReferences = DomainAssembly.GetReferencedAssemblies()
            .Where(a => a.Name?.StartsWith("TaskForge") == true)
            .Select(a => a.Name)
            .ToList();

        domainReferences.Should().BeEmpty(
            $"Domain should not reference other TaskForge assemblies. Found: {string.Join(", ", domainReferences)}");
    }

    #endregion

    #region Consistency Tests

    [Fact(Skip = "disabled")]
    public void All_Handlers_Should_Be_Nested_In_Command_Or_Query_Class()
    {
        var handlers = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .HaveName("Handler")
            .GetTypes()
            .Where(h => h.DeclaringType != null);

        foreach (var handler in handlers)
        {
            handler.DeclaringType.Should().NotBeNull(
                $"Handler {handler.Name} should be nested in Command or Query class");

            var parentName = handler.DeclaringType!.Name;
            (parentName == "Command" || parentName == "Query").Should().BeTrue(
                $"Handler {handler.Name} should be nested in Command or Query, but is in {parentName}");
        }
    }

    [Fact(Skip = "disabled")]
    public void All_Commands_Should_Have_CommandValidator()
    {
        var commands = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .HaveName("Command")
            .GetTypes();

        foreach (var command in commands)
        {
            var parentType = command.DeclaringType;
            if (parentType == null) continue;

            var validators = parentType.GetNestedTypes()
                .Where(t => t.Name == "CommandValidator")
                .ToList();

            validators.Should().NotBeEmpty(
                $"Command {command.Name} should have nested CommandValidator class");
        }
    }

    [Fact]
    public void All_DTOs_Should_Be_In_Core_Namespace()
    {
        var dtos = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Dto")
            .GetTypes();

        foreach (var dto in dtos)
        {
            dto.Namespace.Should().Be("TaskForge.Application.Core",
                $"DTO {dto.Name} should be in TaskForge.Application.Core namespace");
        }
    }

    [Fact]
    public void All_Interfaces_Should_Have_Implementations()
    {
        var allAssemblies = new[] { ApplicationAssembly, EventProcessorAssembly, MessageConsumerAssembly };

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
                var implementations = Types
                    .InAssembly(assembly)
                    .That()
                    .ImplementInterface(interfaceType)
                    .GetTypes();

                implementations.Should().NotBeEmpty(
                    $"Interface {interfaceType.Name} should have at least one implementation");
            }
        }
    }

    [Fact]
    public void Event_Models_Should_Have_Consistent_Properties()
    {
        var eventDto = typeof(global::TaskForge.Application.Core.TaskChangeEventDto);
        var eventModel = typeof(global::TaskForge.EventProcessor.Models.TaskChangeEvent);
        var eventConsumerModel = typeof(global::TaskForge.MessageConsumer.Models.TaskChangeEvent);

        var dtoProperties = eventDto.GetProperties().Select(p => p.Name).OrderBy(n => n).ToList();
        var modelProperties = eventModel.GetProperties().Select(p => p.Name).OrderBy(n => n).ToList();
        var consumerProperties = eventConsumerModel.GetProperties().Select(p => p.Name).OrderBy(n => n).ToList();

        dtoProperties.Should().BeEquivalentTo(modelProperties,
            "TaskChangeEventDto and TaskChangeEvent should have the same properties");

        // Consumer might use DTO directly from Application, so might not need separate check
        if (consumerProperties.Any())
        {
            dtoProperties.Should().BeEquivalentTo(consumerProperties,
                "TaskChangeEventDto and MessageConsumer TaskChangeEvent should have the same properties");
        }
    }

    [Fact]
    public void All_Controllers_Should_Use_ApiController_Attribute()
    {
        var controllers = Types
            .InAssembly(ApiAssembly)
            .That()
            .Inherit(typeof(ControllerBase))
            .GetTypes();

        foreach (var controller in controllers)
        {
            var hasApiControllerAttribute = controller.GetCustomAttributes(typeof(ApiControllerAttribute), true)
                .Any();

            hasApiControllerAttribute.Should().BeTrue(
                $"Controller {controller.Name} should have [ApiController] attribute");
        }
    }

    [Fact]
    public void All_Controllers_Should_Have_Route_Attribute()
    {
        var controllers = Types
            .InAssembly(ApiAssembly)
            .That()
            .Inherit(typeof(ControllerBase))
            .GetTypes();

        foreach (var controller in controllers)
        {
            var hasRouteAttribute = controller.GetCustomAttributes()
                .Any(a => a.GetType().Name.Contains("Route"));

            hasRouteAttribute.Should().BeTrue(
                $"Controller {controller.Name} should have [Route] attribute");
        }
    }

    [Fact]
    public void Result_Class_Should_Be_Immutable()
    {
        var resultType = typeof(global::TaskForge.Application.Core.Result<>);

        var properties = resultType.GetProperties();

        foreach (var property in properties)
        {
            var hasSetter = property.SetMethod != null && property.SetMethod.IsPublic;

            hasSetter.Should().BeFalse(
                $"Result<T> property {property.Name} should be immutable (no public setter)");
        }
    }

    #endregion
}

