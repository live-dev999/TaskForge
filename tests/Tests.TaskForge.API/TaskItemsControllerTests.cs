/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

using Application.TaskItems;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TaskForge.API.Controllers;
using TaskForge.Application.Core;
using TaskForge.Application.TaskItems;
using TaskForge.Domain;
using TaskForge.Domain.Enum;
using Xunit;

namespace Tests.TaskForge.API;

/// <summary>
/// Unit tests for TaskItemsController
/// </summary>
public class TaskItemsControllerTests
{
    #region Helper Methods

    private TaskItemsController CreateController(IMediator mediator)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton(mediator)
            .BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        var controller = new TaskItemsController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        return controller;
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

    #region GetTaskItems Tests

    [Fact]
    public async Task GetTaskItems_WhenCalled_ReturnsOkResult()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskItems = new List<TaskItem> { CreateValidTaskItem() };
        var result = Result<List<TaskItem>>.Success(taskItems);

        mockMediator
            .Setup(m => m.Send(It.IsAny<List.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        var actionResult = await controller.GetTaskItems(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(taskItems, okResult.Value);
    }

    [Fact]
    public async Task GetTaskItems_WhenCalled_SendsListQuery()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskItems = new List<TaskItem> { CreateValidTaskItem() };
        var result = Result<List<TaskItem>>.Success(taskItems);

        mockMediator
            .Setup(m => m.Send(It.IsAny<List.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        await controller.GetTaskItems(CancellationToken.None);

        // Assert
        mockMediator.Verify(m => m.Send(It.IsAny<List.Query>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTaskItems_WhenMediatorReturnsFailure_ReturnsBadRequest()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var result = Result<List<TaskItem>>.Failure("Error occurred");

        mockMediator
            .Setup(m => m.Send(It.IsAny<List.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        var actionResult = await controller.GetTaskItems(CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Error occurred", badRequestResult.Value);
    }

    [Fact]
    public async Task GetTaskItems_WhenCancellationTokenIsCancelled_PropagatesCancellation()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        mockMediator
            .Setup(m => m.Send(It.IsAny<List.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var controller = CreateController(mockMediator.Object);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => 
            controller.GetTaskItems(cancellationTokenSource.Token));
    }

    #endregion

    #region GetTaskItem Tests

    [Fact]
    public async Task GetTaskItem_WhenTaskItemExists_ReturnsOkResult()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskItem = CreateValidTaskItem();
        var result = Result<TaskItem>.Success(taskItem);

        mockMediator
            .Setup(m => m.Send(It.Is<Details.Query>(q => q.Id == taskItem.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        var actionResult = await controller.GetTaskItem(taskItem.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(taskItem, okResult.Value);
    }

    [Fact]
    public async Task GetTaskItem_WhenTaskItemNotFound_ReturnsBadRequest()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var result = Result<TaskItem>.Failure("Task item not found");

        mockMediator
            .Setup(m => m.Send(It.IsAny<Details.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        var actionResult = await controller.GetTaskItem(Guid.NewGuid());

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Task item not found", badRequestResult.Value);
    }

    [Fact]
    public async Task GetTaskItem_WhenCalled_SendsDetailsQueryWithCorrectId()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskId = Guid.NewGuid();
        var result = Result<TaskItem>.Success(null);

        mockMediator
            .Setup(m => m.Send(It.IsAny<Details.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        await controller.GetTaskItem(taskId);

        // Assert
        mockMediator.Verify(m => m.Send(
            It.Is<Details.Query>(q => q.Id == taskId), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    #endregion

    #region CreateTaskItem Tests

    [Fact]
    public async Task CreateTaskItem_WhenValidTaskItem_ReturnsOkResult()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskItem = CreateValidTaskItem();
        var result = Result<Unit>.Success(Unit.Value);

        mockMediator
            .Setup(m => m.Send(It.Is<Create.Command>(c => c.TaskItem == taskItem), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        var actionResult = await controller.CreateTaskItem(taskItem);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(Unit.Value, okResult.Value);
    }

    [Fact]
    public async Task CreateTaskItem_WhenCalled_SendsCreateCommand()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskItem = CreateValidTaskItem();
        var result = Result<Unit>.Success(Unit.Value);

        mockMediator
            .Setup(m => m.Send(It.IsAny<Create.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        await controller.CreateTaskItem(taskItem);

        // Assert
        mockMediator.Verify(m => m.Send(
            It.Is<Create.Command>(c => c.TaskItem == taskItem), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task CreateTaskItem_WhenMediatorReturnsFailure_ReturnsBadRequest()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskItem = CreateValidTaskItem();
        var result = Result<Unit>.Failure("Validation failed");

        mockMediator
            .Setup(m => m.Send(It.IsAny<Create.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        var actionResult = await controller.CreateTaskItem(taskItem);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Validation failed", badRequestResult.Value);
    }

    #endregion

    #region EditTaskItem Tests

    [Fact]
    public async Task EditTaskItem_WhenValidTaskItem_ReturnsOkResult()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskItem = CreateValidTaskItem();
        var taskId = taskItem.Id;
        var result = Result<Unit>.Success(Unit.Value);

        mockMediator
            .Setup(m => m.Send(It.IsAny<Edit.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        var actionResult = await controller.EditTaskItem(taskId, taskItem);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(Unit.Value, okResult.Value);
    }

    [Fact]
    public async Task EditTaskItem_WhenCalled_SetsTaskItemId()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskItem = CreateValidTaskItem();
        var taskId = Guid.NewGuid();
        var result = Result<Unit>.Success(Unit.Value);

        Edit.Command capturedCommand = null;
        mockMediator
            .Setup(m => m.Send(It.IsAny<Edit.Command>(), It.IsAny<CancellationToken>()))
            .Callback<Edit.Command, CancellationToken>((cmd, ct) => 
            {
                capturedCommand = cmd;
            })
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        await controller.EditTaskItem(taskId, taskItem);

        // Assert
        Assert.NotNull(capturedCommand);
        Assert.Equal(taskId, capturedCommand.TaskItem.Id);
    }

    [Fact]
    public async Task EditTaskItem_WhenMediatorReturnsFailure_ReturnsBadRequest()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskItem = CreateValidTaskItem();
        var result = Result<Unit>.Failure("Update failed");

        mockMediator
            .Setup(m => m.Send(It.IsAny<Edit.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        var actionResult = await controller.EditTaskItem(taskItem.Id, taskItem);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Update failed", badRequestResult.Value);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenTaskItemExists_ReturnsOkResult()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskId = Guid.NewGuid();
        var result = Result<Unit>.Success(Unit.Value);

        mockMediator
            .Setup(m => m.Send(It.Is<Delete.Command>(c => c.Id == taskId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        var actionResult = await controller.DeleteAsync(taskId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(Unit.Value, okResult.Value);
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_SendsDeleteCommandWithCorrectId()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskId = Guid.NewGuid();
        var result = Result<Unit>.Success(Unit.Value);

        mockMediator
            .Setup(m => m.Send(It.IsAny<Delete.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        await controller.DeleteAsync(taskId);

        // Assert
        mockMediator.Verify(m => m.Send(
            It.Is<Delete.Command>(c => c.Id == taskId), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenMediatorReturnsFailure_ReturnsBadRequest()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskId = Guid.NewGuid();
        var result = Result<Unit>.Failure("Delete failed");

        mockMediator
            .Setup(m => m.Send(It.IsAny<Delete.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        var actionResult = await controller.DeleteAsync(taskId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Delete failed", badRequestResult.Value);
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsEmpty_SendsDeleteCommandWithEmptyGuid()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskId = Guid.Empty;
        var result = Result<Unit>.Failure("Not found");

        mockMediator
            .Setup(m => m.Send(It.IsAny<Delete.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        await controller.DeleteAsync(taskId);

        // Assert
        mockMediator.Verify(m => m.Send(
            It.Is<Delete.Command>(c => c.Id == Guid.Empty), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task GetTaskItem_WhenIdIsEmpty_SendsQueryWithEmptyGuid()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var result = Result<TaskItem>.Success(null);

        mockMediator
            .Setup(m => m.Send(It.IsAny<Details.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        await controller.GetTaskItem(Guid.Empty);

        // Assert
        mockMediator.Verify(m => m.Send(
            It.Is<Details.Query>(q => q.Id == Guid.Empty), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task CreateTaskItem_WhenTaskItemIsNull_SendsCommandWithNullTaskItem()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var result = Result<Unit>.Failure("Task item is required");

        mockMediator
            .Setup(m => m.Send(It.IsAny<Create.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        await controller.CreateTaskItem(null);

        // Assert
        mockMediator.Verify(m => m.Send(
            It.Is<Create.Command>(c => c.TaskItem == null), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task EditTaskItem_WhenTaskItemIsNull_SetsNullTaskItemWithId()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskId = Guid.NewGuid();
        var result = Result<Unit>.Failure("Task item is required");

        Edit.Command capturedCommand = null;
        mockMediator
            .Setup(m => m.Send(It.IsAny<Edit.Command>(), It.IsAny<CancellationToken>()))
            .Callback<Edit.Command, CancellationToken>((cmd, ct) => 
            {
                capturedCommand = cmd;
            })
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        await controller.EditTaskItem(taskId, null);

        // Assert
        Assert.NotNull(capturedCommand);
        Assert.Null(capturedCommand.TaskItem);
        // Note: In actual code, TaskItems.Id = id would throw NullReferenceException
    }

    #endregion
}

