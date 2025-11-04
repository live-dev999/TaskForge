/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 */

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

    private TaskItemDto CreateValidTaskItem()
    {
        return new TaskItemDto
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
        var taskItems = new List<TaskItemDto> { CreateValidTaskItem() };
        var pagedList = new PagedList<TaskItemDto>(taskItems, taskItems.Count, 1, 10);
        var result = Result<PagedList<TaskItemDto>>.Success(pagedList);

        mockMediator
            .Setup(m => m.Send(It.IsAny<List.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);
        var pagingParams = new PagingParams { PageNumber = 1, PageSize = 10 };

        // Act
        var actionResult = await controller.GetTaskItems(pagingParams, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedPagedList = Assert.IsType<PagedList<TaskItemDto>>(okResult.Value);
        Assert.Equal(taskItems.Count, returnedPagedList.Count);
    }

    [Fact]
    public async Task GetTaskItems_WhenCalled_SendsListQuery()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskItems = new List<TaskItemDto> { CreateValidTaskItem() };
        var pagedList = new PagedList<TaskItemDto>(taskItems, taskItems.Count, 1, 10);
        var result = Result<PagedList<TaskItemDto>>.Success(pagedList);

        mockMediator
            .Setup(m => m.Send(It.IsAny<List.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);
        var pagingParams = new PagingParams { PageNumber = 1, PageSize = 10 };

        // Act
        await controller.GetTaskItems(pagingParams, CancellationToken.None);

        // Assert
        mockMediator.Verify(m => m.Send(It.IsAny<List.Query>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTaskItems_WhenMediatorReturnsFailure_ReturnsBadRequest()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var result = Result<PagedList<TaskItemDto>>.Failure("Error occurred");

        mockMediator
            .Setup(m => m.Send(It.IsAny<List.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);
        var pagingParams = new PagingParams { PageNumber = 1, PageSize = 10 };

        // Act
        var actionResult = await controller.GetTaskItems(pagingParams, CancellationToken.None);

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
        var pagingParams = new PagingParams { PageNumber = 1, PageSize = 10 };

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => 
            controller.GetTaskItems(pagingParams, cancellationTokenSource.Token));
    }

    #endregion

    #region GetTaskItemDto Tests

    [Fact]
    public async Task GetTaskItem_WhenTaskItemExists_ReturnsOkResult()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskItem = CreateValidTaskItem();
        // Details.Query returns TaskItem (domain entity), not TaskItemDto
        var taskItemDomain = new TaskItem
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            Status = taskItem.Status,
            CreatedAt = taskItem.CreatedAt,
            UpdatedAt = taskItem.UpdatedAt
        };
        var result = Result<TaskItem>.Success(taskItemDomain);

        mockMediator
            .Setup(m => m.Send(It.Is<Details.Query>(q => q.Id == taskItem.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        var actionResult = await controller.GetTaskItem(taskItem.Id, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedTaskItem = Assert.IsType<TaskItem>(okResult.Value);
        Assert.Equal(taskItem.Id, returnedTaskItem.Id);
        Assert.Equal(taskItem.Title, returnedTaskItem.Title);
    }

    [Fact]
    public async Task GetTaskItem_WhenTaskItemNotFound_ReturnsNotFound()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        // Details handler now returns Success(null) for not found items
        var result = Result<TaskItem>.Success(null);

        mockMediator
            .Setup(m => m.Send(It.IsAny<Details.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        var actionResult = await controller.GetTaskItem(Guid.NewGuid(), CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(actionResult);
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
        await controller.GetTaskItem(taskId, CancellationToken.None);

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
        var result = Result<TaskItemDto>.Success(taskItem);

        mockMediator
            .Setup(m => m.Send(It.Is<Create.Command>(c => c.TaskItem != null && c.TaskItem.Id == taskItem.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        var actionResult = await controller.CreateTaskItem(taskItem, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedTaskItem = Assert.IsType<TaskItemDto>(okResult.Value);
        Assert.Equal(taskItem.Id, returnedTaskItem.Id);
        Assert.Equal(taskItem.Title, returnedTaskItem.Title);
    }

    [Fact]
    public async Task CreateTaskItem_WhenCalled_SendsCreateCommand()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskItem = CreateValidTaskItem();
        var result = Result<TaskItemDto>.Success(taskItem);

        mockMediator
            .Setup(m => m.Send(It.IsAny<Create.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        await controller.CreateTaskItem(taskItem, CancellationToken.None);

        // Assert
        mockMediator.Verify(m => m.Send(
            It.Is<Create.Command>(c => c.TaskItem != null && c.TaskItem.Id == taskItem.Id && c.TaskItem.Title == taskItem.Title), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task CreateTaskItem_WhenMediatorReturnsFailure_ReturnsBadRequest()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskItem = CreateValidTaskItem();
        var result = Result<TaskItemDto>.Failure("Validation failed");

        mockMediator
            .Setup(m => m.Send(It.IsAny<Create.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        var actionResult = await controller.CreateTaskItem(taskItem, CancellationToken.None);

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
        var actionResult = await controller.EditTaskItem(taskId, taskItem, CancellationToken.None);

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
        var originalTaskItemId = taskItem.Id;
        var taskId = Guid.NewGuid();
        
        var result = Result<Unit>.Success(Unit.Value);

        Edit.Command capturedCommand = null;
        mockMediator
            .Setup(m => m.Send(It.IsAny<IRequest<Result<Unit>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IRequest<Result<Unit>> request, CancellationToken ct) =>
            {
                if (request is Edit.Command cmd)
                {
                    capturedCommand = cmd;
                }
                return result;
            });

        var controller = CreateController(mockMediator.Object);

        // Act
        await controller.EditTaskItem(taskId, taskItem, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedCommand);
        Assert.NotNull(capturedCommand.TaskItem);
        // Controller sets TaskItem.Id = id before sending command
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
        var actionResult = await controller.EditTaskItem(taskItem.Id, taskItem, CancellationToken.None);

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
        var actionResult = await controller.DeleteAsync(taskId, CancellationToken.None);

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
        await controller.DeleteAsync(taskId, CancellationToken.None);

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
        var actionResult = await controller.DeleteAsync(taskId, CancellationToken.None);

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
        await controller.DeleteAsync(taskId, CancellationToken.None);

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
        await controller.GetTaskItem(Guid.Empty, CancellationToken.None);

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
        var result = Result<TaskItemDto>.Failure("Task item is required");

        mockMediator
            .Setup(m => m.Send(It.IsAny<Create.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var controller = CreateController(mockMediator.Object);

        // Act
        await controller.CreateTaskItem(null, CancellationToken.None);

        // Assert
        mockMediator.Verify(m => m.Send(
            It.Is<Create.Command>(c => c.TaskItem == null), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task EditTaskItem_WhenTaskItemIsNull_ThrowsNullReferenceException()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var taskId = Guid.NewGuid();

        var controller = CreateController(mockMediator.Object);

        // Act & Assert
        // In actual code, TaskItem.Id = id would throw NullReferenceException when TaskItem is null
        await Assert.ThrowsAsync<NullReferenceException>(() => 
            controller.EditTaskItem(taskId, null, CancellationToken.None));
        
        // Verify mediator was not called due to exception
        mockMediator.Verify(m => m.Send(It.IsAny<Edit.Command>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion
}

