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
        public async Task CreateToDoTask_ReturnsBadRequestWhenTitleIsMissing()
        {
            // Arrange
            var newTask = new ToDoTask { Id = 0, Title = "", Description = "Description with missing title" }; // Empty title

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", newTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            // Optionally, you can assert on the response body to check for validation errors
            // var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            // Assert.NotNull(errors);
            // Assert.Contains("Title", errors.Errors.Keys);
        }

        [Fact]
        public async Task CreateToDoTask_ReturnsBadRequestWhenTitleIsTooShort()
        {
            // Arrange
            var newTask = new ToDoTask { Id = 0, Title = "ab", Description = "Description with too short title" }; // 2 characters

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", newTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            // Optionally, you can assert on the response body to check for validation errors
        }

        [Fact]
        public async Task CreateToDoTask_ReturnsBadRequestWhenTitleIsTooLong()
        {
            // Arrange
            var longTitle = new string('a', 101); // 101 characters, max is 100
            var newTask = new ToDoTask { Id = 0, Title = longTitle, Description = "Description with too long title" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", newTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            // Optionally, assert on the response body for validation errors
        }

        [Fact]
        public async Task CreateToDoTask_ReturnsBadRequestWhenDescriptionIsTooLong()
        {
            // Arrange
            var longDescription = new string('a', 251); // 251 characters, max is 250
            var newTask = new ToDoTask { Id = 0, Title = "Valid Title", Description = longDescription };

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", newTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            // Optionally, assert on the response body for validation errors
        }

        [Fact]
        public async Task CreateToDoTask_ReturnsBadRequestWhenPercentCompleteIsLessThanZero()
        {
            // Arrange
            var newTask = new ToDoTask { Id = 0, Title = "Valid Title", Description = "Valid Description", PercentComplete = -0.1M };

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", newTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            // Optionally, assert on the response body for validation errors
        }

        [Fact]
        public async Task CreateToDoTask_ReturnsBadRequestWhenPercentCompleteIsGreaterThanOne()
        {
            // Arrange
            var newTask = new ToDoTask { Id = 0, Title = "Valid Title", Description = "Valid Description", PercentComplete = 1.1M };

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", newTask);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            // Optionally, assert on the response body for validation errors
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
        public async Task GetToDoTaskById_ReturnsNotFoundWhenNotExists()
        {
            // Arrange
            var nonExistentId = 999; // An ID that is highly unlikely to exist

            // Act
            var response = await _client.GetAsync($"/api/tasks/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
        public async Task UpdateToDoTask_ReturnsNotFoundWhenNotExists()
        {
            // Arrange
            var nonExistentId = 999; // An ID that is highly unlikely to exist
            var nonExistentTask = new ToDoTask { Id = nonExistentId, Title = "Non Existent", Description = "Desc" };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tasks/{nonExistentId}", nonExistentTask);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateToDoTask_ReturnsBadRequestWhenTitleIsMissing()
        {
            // Arrange - Create a valid task first
            var originalTask = new ToDoTask { Id = 0, Title = "Original Title", Description = "Original Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", originalTask);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            // Prepare an update with a missing title
            var updatedTaskData = new ToDoTask
            {
                Id = createdTask.Id,
                Title = "", // Missing title
                Description = "Updated Description"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tasks/{createdTask.Id}", updatedTaskData);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            // Optionally, assert on the response body for validation errors
        }

        [Fact]
        public async Task UpdateToDoTask_ReturnsBadRequestWhenTitleIsTooShort()
        {
            // Arrange - Create a valid task first
            var originalTask = new ToDoTask { Id = 0, Title = "Original Title", Description = "Original Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", originalTask);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            // Prepare an update with a too short title
            var updatedTaskData = new ToDoTask
            {
                Id = createdTask.Id,
                Title = "a", // Too short title
                Description = "Updated Description"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tasks/{createdTask.Id}", updatedTaskData);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            // Optionally, assert on the response body for validation errors
        }

        [Fact]
        public async Task UpdateToDoTask_ReturnsBadRequestWhenTitleIsTooLong()
        {
            // Arrange - Create a valid task first
            var originalTask = new ToDoTask { Id = 0, Title = "Original Title", Description = "Original Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", originalTask);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            // Prepare an update with a too long title
            var longTitle = new string('a', 101); // 101 characters, max is 100
            var updatedTaskData = new ToDoTask
            {
                Id = createdTask.Id,
                Title = longTitle, // Too long title
                Description = "Updated Description"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tasks/{createdTask.Id}", updatedTaskData);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            // Optionally, assert on the response body for validation errors
        }

        [Fact]
        public async Task UpdateToDoTask_ReturnsBadRequestWhenDescriptionIsTooLong()
        {
            // Arrange - Create a valid task first
            var originalTask = new ToDoTask { Id = 0, Title = "Original Title", Description = "Original Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", originalTask);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            // Prepare an update with a too long description
            var longDescription = new string('a', 251); // 251 characters, max is 250
            var updatedTaskData = new ToDoTask
            {
                Id = createdTask.Id,
                Title = "Updated Title",
                Description = longDescription // Too long description
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tasks/{createdTask.Id}", updatedTaskData);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            // Optionally, assert on the response body for validation errors
        }

        [Fact]
        public async Task UpdateToDoTask_ReturnsBadRequestWhenPercentCompleteIsLessThanZero()
        {
            // Arrange - Create a valid task first
            var originalTask = new ToDoTask { Id = 0, Title = "Original Title", Description = "Original Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", originalTask);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            // Prepare an update with PercentComplete less than zero
            var updatedTaskData = new ToDoTask
            {
                Id = createdTask.Id,
                Title = "Updated Title",
                Description = "Updated Description",
                PercentComplete = -0.1M
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tasks/{createdTask.Id}", updatedTaskData);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            // Optionally, assert on the response body for validation errors
        }

        [Fact]
        public async Task UpdateToDoTask_ReturnsBadRequestWhenPercentCompleteIsGreaterThanOne()
        {
            // Arrange - Create a valid task first
            var originalTask = new ToDoTask { Id = 0, Title = "Original Title", Description = "Original Description" };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", originalTask);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            // Prepare an update with PercentComplete greater than one
            var updatedTaskData = new ToDoTask
            {
                Id = createdTask.Id,
                Title = "Updated Title",
                Description = "Updated Description",
                PercentComplete = 1.1M
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tasks/{createdTask.Id}", updatedTaskData);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            // Optionally, assert on the response body for validation errors
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
        public async Task DeleteToDoTask_ReturnsNotFoundWhenNotExists()
        {
            // Arrange
            var nonExistentId = 999; // An ID that is highly unlikely to exist

            // Act
            var response = await _client.DeleteAsync($"/api/tasks/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
        public async Task GetIncomingToDoTasks_ReturnsTomorrowTasks()
        {
            // Arrange - Create a task that should be incoming tomorrow
            var newTask = new ToDoTask { Id = 0, Title = "Incoming Task Tomorrow", Description = "Description", ExpiryDate = DateTime.UtcNow.Date.AddDays(1).AddHours(1) };
            await _client.PostAsJsonAsync("/api/tasks", newTask);

            // Act
            var response = await _client.GetAsync("/api/tasks/incoming?incomingTasksFilter=Tomorrow");

            // Assert
            response.EnsureSuccessStatusCode();
            var tasks = await response.Content.ReadFromJsonAsync<IEnumerable<ToDoTask>>();
            Assert.NotNull(tasks);
            Assert.Single(tasks);
            Assert.Contains(tasks, t => t.Title == "Incoming Task Tomorrow");
        }

        [Fact]
        public async Task GetIncomingToDoTasks_ReturnsThisWeekTasks()
        {
            // Arrange - Create tasks for this week (only today's task will be returned by current AppDataProvider logic)
            var todayTask = new ToDoTask { Id = 0, Title = "This Week Task Today", Description = "Desc", ExpiryDate = DateTime.UtcNow.Date.AddHours(1) };
            var nextWeekTask = new ToDoTask { Id = 0, Title = "Next Week Task", Description = "Desc", ExpiryDate = DateTime.UtcNow.Date.AddDays(8).AddHours(1) };

            await _client.PostAsJsonAsync("/api/tasks", todayTask);
            await _client.PostAsJsonAsync("/api/tasks", nextWeekTask);

            // Act
            var response = await _client.GetAsync("/api/tasks/incoming?incomingTasksFilter=ThisWeek");

            // Assert
            response.EnsureSuccessStatusCode();
            var tasks = await response.Content.ReadFromJsonAsync<IEnumerable<ToDoTask>>();
            Assert.NotNull(tasks);
            Assert.Single(tasks);
            Assert.Contains(tasks, t => t.Title == "This Week Task Today");
            Assert.DoesNotContain(tasks, t => t.Title == "Next Week Task");
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
        public async Task SetToDoTaskProgress_ReturnsNotFoundWhenNotExists()
        {
            // Arrange
            var nonExistentId = 999; // An ID that is highly unlikely to exist

            // Act
            var patchResponse = await _client.PatchAsync($"/api/tasks/{nonExistentId}/percent?percent=0.5", null);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, patchResponse.StatusCode);
        }

                [Fact]
        public async Task SetToDoTaskProgress_SetsIsDoneToTrueWhenPercentCompleteIsOne()
        {
            // Arrange - Create a task with PercentComplete less than 1.0M
            var newTask = new ToDoTask { Id = 0, Title = "Task to Complete", Description = "Description", PercentComplete = 0.5M };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", newTask);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            // Act - Set PercentComplete to 1.0M
            var patchResponse = await _client.PatchAsync($"/api/tasks/{createdTask.Id}/percent?percent=1.0", null);

            // Assert
            patchResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, patchResponse.StatusCode);

            // Verify the task is updated and IsDone is true
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

        [Fact]
        public async Task MarkToDoTaskAsDone_ReturnsNotFoundWhenNotExists()
        {
            // Arrange
            var nonExistentId = 999; // An ID that is highly unlikely to exist

            // Act
            var patchResponse = await _client.PatchAsync($"/api/tasks/{nonExistentId}/done", null);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, patchResponse.StatusCode);
        }

        [Fact]
        public async Task MarkToDoTaskAsDone_SetsPercentCompleteToOneAndIsDoneToTrue()
        {
            // Arrange - Create a task that is not done and has some progress
            var newTask = new ToDoTask { Id = 0, Title = "Task to Mark Done", Description = "Description", PercentComplete = 0.5M, IsDone = false };
            var postResponse = await _client.PostAsJsonAsync("/api/tasks", newTask);
            postResponse.EnsureSuccessStatusCode();
            var createdTask = await postResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(createdTask);

            // Act
            var patchResponse = await _client.PatchAsync($"/api/tasks/{createdTask.Id}/done", null);

            // Assert
            patchResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, patchResponse.StatusCode);

            // Verify the task is updated
            var getResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}");
            getResponse.EnsureSuccessStatusCode();
            var updatedTask = await getResponse.Content.ReadFromJsonAsync<ToDoTask>();
            Assert.NotNull(updatedTask);
            Assert.True(updatedTask.IsDone);
            Assert.Equal(1.0M, updatedTask.PercentComplete);
        }
    }
}