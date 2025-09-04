using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using TaskNET.Models;
using Xunit;

namespace TaskNET.Test.IntegrationTests
{
    public class ToDoTaskIntegrationTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task GetToDoTasks_ReturnsSuccessAndCorrectContentType()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/api/tasks");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        }

        [Fact]
        public async Task CreateToDoTask_ReturnsCreatedTask()
        {
            // Arrange
            var newTask = new ToDoTask { Id = 0, Title = "Integration Test Task", Description = "Description for integration test" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", newTask);

            // Assert
            response.EnsureSuccessStatusCode();
            var createdTask = await response.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);
            Assert.Equal(newTask.Title, createdTask.Title);
            Assert.Equal(newTask.Description, createdTask.Description);
        }

        [Fact]
        public async Task GetToDoTaskById_ReturnsTask()
        {
            // Arrange - Create a task first
            var newTask = new ToDoTask { Id = 0, Title = "Task for GetById", Description = "Description for GetById" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", newTask);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            // Act
            var getResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}");

            // Assert
            getResponse.EnsureSuccessStatusCode();
            var fetchedTask = await getResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(fetchedTask);
            Assert.Equal(createdTask.Id, fetchedTask.Id);
            Assert.Equal(createdTask.Title, fetchedTask.Title);
        }

        [Fact]
        public async Task UpdateToDoTask_ReturnsUpdatedTask()
        {
            // Arrange - Create a task first
            var newTask = new ToDoTask { Id = 0, Title = "Task to Update", Description = "Original Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", newTask);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            // Update the task
            createdTask.Description = "Updated Description";

            // Act
            var putResponse = await _client.PutAsJsonAsync($"/api/tasks/{createdTask.Id}", createdTask);

            // Assert
            putResponse.EnsureSuccessStatusCode();
            var updatedTask = await putResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(updatedTask);
            Assert.Equal(createdTask.Description, updatedTask.Description);
        }

        [Fact]
        public async Task DeleteToDoTask_ReturnsNoContent()
        {
            // Arrange - Create a task first
            var newTask = new ToDoTask { Id = 0, Title = "Task to Delete", Description = "Description for Delete" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", newTask);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/tasks/{createdTask.Id}");

            // Assert
            deleteResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verify it's deleted
            var getResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task GetIncomingToDoTasks_ReturnsSuccessAndTasks()
        {
            // Arrange - Create a task that should be incoming (e.g., today)
            var newTask = new ToDoTask { Id = 0, Title = "Incoming Task Today", Description = "Description", ExpiryDate = DateTime.UtcNow.AddHours(1) };
            await _client.PostAsJsonAsync("/api/tasks", newTask);

            // Act
            var response = await _client.GetAsync("/api/tasks/incoming");

            // Assert
            response.EnsureSuccessStatusCode();
            var tasks = await response.Content.ReadFromJsonAsync<IEnumerable<ToDoTask>>();
            Assert.NotNull(tasks);
            Assert.Contains(tasks, t => t.Title == "Incoming Task Today");
        }

        [Fact]
        public async Task SetToDoTaskProgress_ReturnsNoContent()
        {
            // Arrange - Create a task
            var newTask = new ToDoTask { Id = 0, Title = "Task for Progress", Description = "Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", newTask);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            // Act
            var patchResponse = await _client.PatchAsync($"/api/tasks/{createdTask.Id}/percent?percent=0.5", null);

            // Assert
            patchResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, patchResponse.StatusCode);
        }

        [Fact]
        public async Task MarkToDoTaskAsDone_ReturnsNoContent()
        {
            // Arrange - Create a task
            var newTask = new ToDoTask { Id = 0, Title = "Task to Mark Done", Description = "Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", newTask);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            // Act
            var patchResponse = await _client.PatchAsync($"/api/tasks/{createdTask.Id}/done", null);

            // Assert
            patchResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, patchResponse.StatusCode);
        }
    }
}