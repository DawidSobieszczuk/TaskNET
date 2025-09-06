using System.Net;
using System.Net.Http.Json;
using TaskNET.Models;

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
            var newTask = new ToDoTask { Id = 0, Title = "Task", Description = "Description" };

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
        public async Task CreateToDoTask_ReturnsBadRequestWhenTitleIsMissing()
        {
            // Arrange
            var newTask = new ToDoTask { Id = 0, Title = "", Description = "Description" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", newTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateToDoTask_ReturnsBadRequestWhenTitleIsTooShort()
        {
            // Arrange
            var newTask = new ToDoTask { Id = 0, Title = "ab", Description = "Description" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", newTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateToDoTask_ReturnsBadRequestWhenTitleIsTooLong()
        {
            // Arrange
            var longText = new string('a', 101);
            var newTask = new ToDoTask { Id = 0, Title = longText, Description = "Description" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", newTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateToDoTask_ReturnsBadRequestWhenDescriptionIsTooLong()
        {
            // Arrange
            var longText = new string('a', 251);
            var newTask = new ToDoTask { Id = 0, Title = "Task", Description = longText };

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", newTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateToDoTask_ReturnsBadRequestWhenPercentCompleteIsLessThanZero()
        {
            // Arrange
            var newTask = new ToDoTask { Id = 0, Title = "Task", Description = "Description", PercentComplete = -0.1M };

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", newTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateToDoTask_ReturnsBadRequestWhenPercentCompleteIsGreaterThanOne()
        {
            // Arrange
            var newTask = new ToDoTask { Id = 0, Title = "Task", Description = "Description", PercentComplete = 1.1M };

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", newTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetToDoTaskById_ReturnsTask()
        {
            // Arrange
            var newTask = new ToDoTask { Id = 0, Title = "Task", Description = "Description" };
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
        public async Task GetToDoTaskById_ReturnsNotFoundWhenNotExists()
        {
            // Arrange
            var nonExistentId = 999;

            // Act
            var response = await _client.GetAsync($"/api/tasks/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateToDoTask_ReturnsUpdatedTask()
        {
            // Arrange
            var newTask = new ToDoTask { Id = 0, Title = "Task", Description = "Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", newTask);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

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
        public async Task UpdateToDoTask_ReturnsNotFoundWhenNotExists()
        {
            // Arrange
            var nonExistentId = 999;
            var nonExistentTask = new ToDoTask { Id = nonExistentId, Title = "Task", Description = "Description" };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tasks/{nonExistentId}", nonExistentTask);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateToDoTask_ReturnsBadRequestWhenTitleIsMissing()
        {
            // Arrange
            var task = new ToDoTask { Id = 0, Title = "Task", Description = "Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", task);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            createdTask.Title = "";

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tasks/{createdTask.Id}", createdTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateToDoTask_ReturnsBadRequestWhenTitleIsTooShort()
        {
            // Arrange
            var task = new ToDoTask { Id = 0, Title = "Task", Description = "Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", task);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            createdTask.Title = "ab";

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tasks/{createdTask.Id}", createdTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateToDoTask_ReturnsBadRequestWhenTitleIsTooLong()
        {
            // Arrange
            var task = new ToDoTask { Id = 0, Title = "Task", Description = "Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", task);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            var longText = new string('a', 101);
            createdTask.Title = longText;

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tasks/{createdTask.Id}", createdTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateToDoTask_ReturnsBadRequestWhenDescriptionIsTooLong()
        {
            // Arrange
            var task = new ToDoTask { Id = 0, Title = "Task", Description = "Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", task);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            var longText = new string('a', 251);
            createdTask.Description = longText;

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tasks/{createdTask.Id}", createdTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateToDoTask_ReturnsBadRequestWhenPercentCompleteIsLessThanZero()
        {
            // Arrange
            var task = new ToDoTask { Id = 0, Title = "Task", Description = "Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", task);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            createdTask.PercentComplete = -0.1M;

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tasks/{createdTask.Id}", createdTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateToDoTask_ReturnsBadRequestWhenPercentCompleteIsGreaterThanOne()
        {
            // Arrange
            var task = new ToDoTask { Id = 0, Title = "Task", Description = "Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", task);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            createdTask.PercentComplete = 1.1M;

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tasks/{createdTask.Id}", createdTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteToDoTask_ReturnsNoContent()
        {
            // Arrange
            var task = new ToDoTask { Id = 0, Title = "Task", Description = "Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", task);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/tasks/{createdTask.Id}");

            // Assert
            deleteResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteToDoTask_ReturnsNotFoundWhenNotExists()
        {
            // Arrange
            var nonExistentId = 999;

            // Act
            var response = await _client.DeleteAsync($"/api/tasks/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetIncomingToDoTasks_ReturnsSuccessAndTasks()
        {
            // Arrange
            var task = new ToDoTask { Id = 0, Title = "Task", Description = "Description", ExpiryDate = DateTime.UtcNow.AddHours(1) };
            await _client.PostAsJsonAsync("/api/tasks", task);

            // Act
            var response = await _client.GetAsync("/api/tasks/incoming");

            // Assert
            response.EnsureSuccessStatusCode();
            var tasks = await response.Content.ReadFromJsonAsync<IEnumerable<ToDoTask>>();
            Assert.NotNull(tasks);
            Assert.Contains(tasks, t => t.Title == task.Title);
        }

        [Fact]
        public async Task GetIncomingToDoTasks_ReturnsTomorrowTasks()
        {
            // Arrange
            var task = new ToDoTask { Id = 0, Title = "Task", Description = "Description", ExpiryDate = DateTime.UtcNow.Date.AddDays(1).AddHours(1) };
            await _client.PostAsJsonAsync("/api/tasks", task);

            // Act
            var response = await _client.GetAsync("/api/tasks/incoming?incomingTasksFilter=Tomorrow");

            // Assert
            response.EnsureSuccessStatusCode();
            var tasks = await response.Content.ReadFromJsonAsync<IEnumerable<ToDoTask>>();
            Assert.NotNull(tasks);
            Assert.Single(tasks);
            Assert.Contains(tasks, t => t.Title == task.Title);
        }

        [Fact]
        public async Task GetIncomingToDoTasks_ReturnsThisWeekTasks()
        {
            // Arrange
            var todayTask = new ToDoTask { Id = 0, Title = "Task 0", Description = "Description", ExpiryDate = DateTime.UtcNow.Date.AddHours(1) };
            var nextWeekTask = new ToDoTask { Id = 0, Title = "Task 8", Description = "Description", ExpiryDate = DateTime.UtcNow.Date.AddDays(8).AddHours(1) };

            await _client.PostAsJsonAsync("/api/tasks", todayTask);
            await _client.PostAsJsonAsync("/api/tasks", nextWeekTask);

            // Act
            var response = await _client.GetAsync("/api/tasks/incoming?incomingTasksFilter=ThisWeek");

            // Assert
            response.EnsureSuccessStatusCode();
            var tasks = await response.Content.ReadFromJsonAsync<IEnumerable<ToDoTask>>();
            Assert.NotNull(tasks);
            Assert.Single(tasks);
            Assert.Contains(tasks, t => t.Title == todayTask.Title);
            Assert.DoesNotContain(tasks, t => t.Title == nextWeekTask.Title);
        }

        [Fact]
        public async Task SetToDoTaskProgress_ReturnsNoContent()
        {
            // Arrange
            var task = new ToDoTask { Id = 0, Title = "Task", Description = "Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", task);
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
        public async Task SetToDoTaskProgress_ReturnsNotFoundWhenNotExists()
        {
            // Arrange
            var nonExistentId = 999;

            // Act
            var patchResponse = await _client.PatchAsync($"/api/tasks/{nonExistentId}/percent?percent=0.5", null);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, patchResponse.StatusCode);
        }

                [Fact]
        public async Task SetToDoTaskProgress_SetsIsDoneToTrueWhenPercentCompleteIsOne()
        {
            // Arrange
            var task = new ToDoTask { Id = 0, Title = "Task", Description = "Description", PercentComplete = 0.5M };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", task);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            // Act
            var patchResponse = await _client.PatchAsync($"/api/tasks/{createdTask.Id}/percent?percent=1.0", null);

            // Assert
            patchResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, patchResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}");
            getResponse.EnsureSuccessStatusCode();
            var updatedTask = await getResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(updatedTask);
            Assert.Equal(1.0M, updatedTask.PercentComplete);
            Assert.True(updatedTask.IsDone);
        }

        [Fact]
        public async Task MarkToDoTaskAsDone_ReturnsNoContent()
        {
            var task = new ToDoTask { Id = 0, Title = "Task", Description = "Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", task);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            // Act
            var patchResponse = await _client.PatchAsync($"/api/tasks/{createdTask.Id}/done", null);

            // Assert
            patchResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, patchResponse.StatusCode);
        }

        [Fact]
        public async Task MarkToDoTaskAsDone_ReturnsNotFoundWhenNotExists()
        {
            // Arrange
            var nonExistentId = 999;

            // Act
            var patchResponse = await _client.PatchAsync($"/api/tasks/{nonExistentId}/done", null);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, patchResponse.StatusCode);
        }

        [Fact]
        public async Task MarkToDoTaskAsDone_SetsPercentCompleteToOneAndIsDoneToTrue()
        {
            // Arrange
            var newTask = new ToDoTask { Id = 0, Title = "Task", Description = "Description", PercentComplete = 0.5M, IsDone = false };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", newTask);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            // Act
            var patchResponse = await _client.PatchAsync($"/api/tasks/{createdTask.Id}/done", null);

            // Assert
            patchResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, patchResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}");
            getResponse.EnsureSuccessStatusCode();
            var updatedTask = await getResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(updatedTask);
            Assert.True(updatedTask.IsDone);
            Assert.Equal(1.0M, updatedTask.PercentComplete);
        }
    }
}