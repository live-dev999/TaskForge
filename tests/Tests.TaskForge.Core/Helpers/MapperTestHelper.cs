/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using AutoMapper;
using TaskForge.Application.Core;

namespace Tests.TaskForge.Core.Helpers;

/// <summary>
/// Helper methods for creating mapper instances for testing
/// </summary>
public static class MapperTestHelper
{
    /// <summary>
    /// Creates an AutoMapper instance with MappingProfiles configuration
    /// </summary>
    /// <returns>Configured IMapper instance</returns>
    public static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfiles>();
        });
        return configuration.CreateMapper();
    }

    /// <summary>
    /// Creates an AutoMapper instance with custom profile configuration
    /// </summary>
    /// <param name="configure">Action to configure additional profiles</param>
    /// <returns>Configured IMapper instance</returns>
    public static IMapper CreateMapper(Action<IMapperConfigurationExpression> configure)
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfiles>();
            configure(cfg);
        });
        return mapperConfig.CreateMapper();
    }

    /// <summary>
    /// Validates that the mapper configuration is valid
    /// </summary>
    /// <returns>True if configuration is valid</returns>
    public static bool ValidateMapperConfiguration()
    {
        try
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfiles>();
            });
            configuration.AssertConfigurationIsValid();
            return true;
        }
        catch
        {
            return false;
        }
    }
}

