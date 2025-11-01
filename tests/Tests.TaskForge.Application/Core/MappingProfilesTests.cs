/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using AutoMapper;
using TaskForge.Application.Core;
using TaskForge.Domain;
using TaskForge.Domain.Enum;

namespace Tests.TaskForge.Application.Core;

/// <summary>
/// Unit tests for MappingProfiles
/// </summary>
public class MappingProfilesTests
{
    #region Helper Methods

    private IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfiles>();
        });
        return configuration.CreateMapper();
    }

    private TaskItem CreateValidTaskItem()
    {
        return new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Title",
            Description = "Test Description",
            Status = TaskItemStatus.New,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    #endregion

    #region Mapping Configuration Tests

    [Fact]
    public void MappingConfiguration_ShouldBeValid()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfiles>();
        });

        // Act & Assert
        configuration.AssertConfigurationIsValid();
    }

    #endregion

    #region TaskItem to TaskItem Mapping Tests

    [Fact]
    public void Map_WhenMappingTaskItemToTaskItem_MapsAllProperties()
    {
        // Arrange
        var mapper = CreateMapper();
        var source = CreateValidTaskItem();

        // Act
        var destination = mapper.Map<TaskItem>(source);

        // Assert
        Assert.Equal(source.Id, destination.Id);
        Assert.Equal(source.Title, destination.Title);
        Assert.Equal(source.Description, destination.Description);
        Assert.Equal(source.Status, destination.Status);
        Assert.Equal(source.CreatedAt, destination.CreatedAt);
        Assert.Equal(source.UpdatedAt, destination.UpdatedAt);
    }

    [Fact]
    public void Map_WhenMappingTaskItemToTaskItem_CreatesNewInstance()
    {
        // Arrange
        var mapper = CreateMapper();
        var source = CreateValidTaskItem();

        // Act
        var destination = mapper.Map<TaskItem>(source);

        // Assert
        Assert.NotSame(source, destination);
    }

    [Fact]
    public void Map_WhenMappingTaskItemWithAllStatuses_MapsCorrectly()
    {
        // Arrange
        var mapper = CreateMapper();
        var statuses = new[] { TaskItemStatus.New, TaskItemStatus.InProgress, TaskItemStatus.Completed, TaskItemStatus.Pending };

        foreach (var status in statuses)
        {
            var source = CreateValidTaskItem();
            source.Status = status;

            // Act
            var destination = mapper.Map<TaskItem>(source);

            // Assert
            Assert.Equal(status, destination.Status);
        }
    }

    [Fact]
    public void Map_WhenMappingTaskItemWithNullTitle_MapsCorrectly()
    {
        // Arrange
        var mapper = CreateMapper();
        var source = CreateValidTaskItem();
        source.Title = null;

        // Act
        var destination = mapper.Map<TaskItem>(source);

        // Assert
        Assert.Null(destination.Title);
    }

    [Fact]
    public void Map_WhenMappingTaskItemWithEmptyGuid_MapsCorrectly()
    {
        // Arrange
        var mapper = CreateMapper();
        var source = CreateValidTaskItem();
        source.Id = Guid.Empty;

        // Act
        var destination = mapper.Map<TaskItem>(source);

        // Assert
        Assert.Equal(Guid.Empty, destination.Id);
    }

    [Fact]
    public void Map_WhenMappingTaskItemWithMinDateTime_MapsCorrectly()
    {
        // Arrange
        var mapper = CreateMapper();
        var source = CreateValidTaskItem();
        source.CreatedAt = DateTime.MinValue;
        source.UpdatedAt = DateTime.MinValue;

        // Act
        var destination = mapper.Map<TaskItem>(source);

        // Assert
        Assert.Equal(DateTime.MinValue, destination.CreatedAt);
        Assert.Equal(DateTime.MinValue, destination.UpdatedAt);
    }

    [Fact]
    public void Map_WhenMappingTaskItemWithMaxDateTime_MapsCorrectly()
    {
        // Arrange
        var mapper = CreateMapper();
        var source = CreateValidTaskItem();
        source.CreatedAt = DateTime.MaxValue;
        source.UpdatedAt = DateTime.MaxValue;

        // Act
        var destination = mapper.Map<TaskItem>(source);

        // Assert
        Assert.Equal(DateTime.MaxValue, destination.CreatedAt);
        Assert.Equal(DateTime.MaxValue, destination.UpdatedAt);
    }

    [Fact]
    public void Map_WhenMappingTaskItemToExistingInstance_UpdatesProperties()
    {
        // Arrange
        var mapper = CreateMapper();
        var source = CreateValidTaskItem();
        var destination = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Old Title"
        };

        // Act
        mapper.Map(source, destination);

        // Assert
        Assert.Equal(source.Id, destination.Id);
        Assert.Equal(source.Title, destination.Title);
        Assert.Equal(source.Description, destination.Description);
        Assert.Equal(source.Status, destination.Status);
    }

    [Fact]
    public void Map_WhenMappingTaskItemWithVeryLongStrings_MapsCorrectly()
    {
        // Arrange
        var mapper = CreateMapper();
        var source = CreateValidTaskItem();
        source.Title = new string('A', 10000);
        source.Description = new string('B', 50000);

        // Act
        var destination = mapper.Map<TaskItem>(source);

        // Assert
        Assert.Equal(source.Title, destination.Title);
        Assert.Equal(source.Description, destination.Description);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Map_WhenMappingNullTaskItem_ThrowsArgumentNullException()
    {
        // Arrange
        var mapper = CreateMapper();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map<TaskItem>(null));
    }

    [Fact]
    public void Map_WhenMappingToNullDestination_ThrowsArgumentNullException()
    {
        // Arrange
        var mapper = CreateMapper();
        var source = CreateValidTaskItem();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map<TaskItem>(source, null));
    }

    #endregion
}

