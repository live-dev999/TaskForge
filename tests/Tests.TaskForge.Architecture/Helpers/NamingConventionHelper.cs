/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using System.Reflection;
using NetArchTest.Rules;

namespace Tests.TaskForge.Architecture.Helpers;

/// <summary>
/// Helper methods for checking naming conventions.
/// </summary>
public static class NamingConventionHelper
{
    /// <summary>
    /// Checks if private fields start with underscore.
    /// </summary>
    public static List<string> FindPrivateFieldsWithoutUnderscore(IEnumerable<Assembly> assemblies)
    {
        var violations = new List<string>();

        foreach (var assembly in assemblies)
        {
            var types = Types.InAssembly(assembly)
                .That()
                .AreClasses()
                .GetTypes()
                .Where(t => !t.Name.Equals("Program") && !t.Name.Equals("Startup"));

            foreach (var type in types)
            {
                var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(f => !f.Name.StartsWith("_") && !f.IsLiteral);

                violations.AddRange(fields.Select(f => $"{type.FullName}.{f.Name}"));
            }
        }

        return violations;
    }

    /// <summary>
    /// Checks if public members use PascalCase.
    /// </summary>
    public static List<string> FindPublicMembersNotPascalCase(IEnumerable<Assembly> assemblies)
    {
        var violations = new List<string>();

        foreach (var assembly in assemblies)
        {
            var types = Types.InAssembly(assembly)
                .That()
                .AreClasses()
                .GetTypes()
                .Where(t => !t.Name.Equals("Program") && !t.Name.Equals("Startup"));

            foreach (var type in types)
            {
                var members = type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => (m is PropertyInfo || m is MethodInfo) &&
                               !m.Name.StartsWith("get_") &&
                               !m.Name.StartsWith("set_") &&
                               !m.Name.StartsWith("op_") &&
                               !m.Name.StartsWith("add_") &&
                               !m.Name.StartsWith("remove_") &&
                               !char.IsUpper(m.Name[0]));

                violations.AddRange(members.Select(m => $"{type.FullName}.{m.Name}"));
            }
        }

        return violations;
    }

    /// <summary>
    /// Checks if async methods end with Async suffix.
    /// </summary>
    public static List<string> FindAsyncMethodsWithoutAsyncSuffix(IEnumerable<Assembly> assemblies)
    {
        var violations = new List<string>();

        foreach (var assembly in assemblies)
        {
            var types = Types.InAssembly(assembly)
                .That()
                .AreClasses()
                .GetTypes()
                .Where(t => !t.Name.Equals("Program") && !t.Name.Equals("Startup"));

            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                    .Where(m => m.IsDefined(typeof(System.Runtime.CompilerServices.AsyncStateMachineAttribute)) &&
                               !m.Name.EndsWith("Async") &&
                               !m.Name.Contains("<") &&
                               !m.IsSpecialName);

                violations.AddRange(methods.Select(m => $"{type.FullName}.{m.Name}"));
            }
        }

        return violations;
    }

    /// <summary>
    /// Checks if interfaces start with I.
    /// </summary>
    public static List<string> FindInterfacesNotStartingWithI(IEnumerable<Assembly> assemblies)
    {
        var violations = new List<string>();

        foreach (var assembly in assemblies)
        {
            var interfaces = Types.InAssembly(assembly)
                .That()
                .AreInterfaces()
                .GetTypes()
                .Where(i => !i.Name.StartsWith("I"));

            violations.AddRange(interfaces.Select(i => i.FullName ?? i.Name));
        }

        return violations;
    }
}

