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

using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskForge.Domain;
using TaskForge.Domain.Enum;
using TaskForge.Persistence;

namespace IntegratonTests.TaskForge.API;

/// <summary>
/// Integration test for TaskItemsController - full CRUD operations workflow
/// </summary>
public class TaskItemsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly DataContext _context;
    private readonly IServiceScope _scope;

    public TaskItemsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the real database context registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DataContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing
                services.AddDbContext<DataContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase_" + Guid.NewGuid().ToString());
                });
            });
        });

        _client = _factory.CreateClient();
        
        // Create a scope to access services
        _scope = _factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<DataContext>();
    }

    [Fact]
    public async Task TaskItemsController_FullCrudWorkflow_WorksCorrectly()
    {
        // Arrange - Create a task item
        var createTaskItem = new TaskItem
        {
            Title = "Integration Test Task",
            Description = "This is a test task for integration testing",
            Status = TaskItemStatus.New
        };

        // Act 1 - Create TaskItem
        var createResponse = await _client.PostAsJsonAsync("/api/taskitems", createTaskItem);

        // Assert 1 - Verify creation
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        
        var createdTaskItem = await createResponse.Content.ReadFromJsonAsync<TaskItem>();
        Assert.NotNull(createdTaskItem);
        Assert.NotEqual(Guid.Empty, createdTaskItem.Id);
        Assert.Equal(createTaskItem.Title, createdTaskItem.Title);
        Assert.Equal(createTaskItem.Description, createdTaskItem.Description);
        Assert.Equal(createTaskItem.Status, createdTaskItem.Status);
        Assert.True(createdTaskItem.CreatedAt > DateTime.MinValue);
        Assert.True(createdTaskItem.UpdatedAt > DateTime.MinValue);

        // Verify it was saved to database
        var savedTaskItem = await _context.TaskItems.FindAsync(createdTaskItem.Id);
        Assert.NotNull(savedTaskItem);
        Assert.Equal(createTaskItem.Title, savedTaskItem.Title);

        var taskId = createdTaskItem.Id;

        // Act 2 - Get TaskItem by ID
        var getResponse = await _client.GetAsync($"/api/taskitems/{taskId}");

        // Assert 2 - Verify retrieval
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        
        var retrievedTaskItem = await getResponse.Content.ReadFromJsonAsync<TaskItem>();
        Assert.NotNull(retrievedTaskItem);
        Assert.Equal(taskId, retrievedTaskItem.Id);
        Assert.Equal(createTaskItem.Title, retrievedTaskItem.Title);

        // Act 3 - Update TaskItem
        var updatedTaskItem = new TaskItem
        {
            Title = "Updated Integration Test Task",
            Description = "Updated description",
            Status = TaskItemStatus.InProgress
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/taskitems/{taskId}", updatedTaskItem);

        // Assert 3 - Verify update
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var savedAfterUpdate = await _context.TaskItems.FindAsync(taskId);
        Assert.NotNull(savedAfterUpdate);
        Assert.Equal(updatedTaskItem.Title, savedAfterUpdate.Title);
        Assert.Equal(updatedTaskItem.Description, savedAfterUpdate.Description);
        Assert.Equal(updatedTaskItem.Status, savedAfterUpdate.Status);
        Assert.Equal(createdTaskItem.CreatedAt, savedAfterUpdate.CreatedAt); // CreatedAt should be preserved
        Assert.True(savedAfterUpdate.UpdatedAt > createdTaskItem.UpdatedAt); // UpdatedAt should be changed

        // Act 4 - Get all TaskItems
        var getAllResponse = await _client.GetAsync("/api/taskitems");

        // Assert 4 - Verify list contains our task
        Assert.Equal(HttpStatusCode.OK, getAllResponse.StatusCode);
        
        var allTaskItems = await getAllResponse.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.NotNull(allTaskItems);
        Assert.Contains(allTaskItems, t => t.Id == taskId);

        // Act 5 - Delete TaskItem
        var deleteResponse = await _client.DeleteAsync($"/api/taskitems/{taskId}");

        // Assert 5 - Verify deletion
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var deletedTaskItem = await _context.TaskItems.FindAsync(taskId);
        Assert.Null(deletedTaskItem);

        // Act 6 - Try to get deleted TaskItem
        var getDeletedResponse = await _client.GetAsync($"/api/taskitems/{taskId}");

        // Assert 6 - Verify not found
        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }

    public void Dispose()
    {
        _context?.Database.EnsureDeleted();
        _context?.Dispose();
        _scope?.Dispose();
        _client?.Dispose();
    }
}


