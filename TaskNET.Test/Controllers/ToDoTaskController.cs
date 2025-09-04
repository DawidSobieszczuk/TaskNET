using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskNET.Interfaces;
using TaskNET.Models;

namespace TaskNET.Test.Controllers
{
    public class ToDoTaskController
    {
        private readonly Mock<IAppDataProvider> _mockDataProvider;
        private readonly TaskNET.Controllers.ToDoTaskController _controller;

        public ToDoTaskController()
        {
            _mockDataProvider = new Mock<IAppDataProvider>();
            _controller = new TaskNET.Controllers.ToDoTaskController(_mockDataProvider.Object);
        }

        [Fact]
        public async Task GetToDoTasks_ReturnsOkObjectResultAndTasks()
        {
            // Arrange
            _mockDataProvider.Setup(m => m.GetToDoTasksAsync())
                .ReturnsAsync(
                [
                    new() { Id = 1, Title = "Test Task 1", Description = "Description 1", ExpiryDate = DateTime.UtcNow.AddDays(1) },
                    new() { Id = 2, Title = "Test Task 2", Description = "Description 2", ExpiryDate = DateTime.UtcNow.AddDays(2) }
                ]);

            // Act
            var result = await _controller.GetToDoTasks();
            var okResult = result as OkObjectResult;
            var tasks = okResult?.Value as IEnumerable<ToDoTask>;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult?.StatusCode);
            Assert.NotNull(tasks);
            Assert.Equal(2, tasks.Count());
        }

        [Fact]
        public async Task GetToDoTask_WithExistingId_ReturnsOkObjectResultWithTask()
        {
            // Arrange
            var id = 1;
            var expectedTask = new ToDoTask { Id = id, Title = "Task", Description = "Description", ExpiryDate = DateTime.UtcNow.AddDays(1) };
            _mockDataProvider.Setup(m => m.GetToDoTaskAsync(1)).ReturnsAsync(expectedTask);

            // Act
            var result = await _controller.GetToDoTask(id);
            var okResult = result as OkObjectResult;
            var todoTask = okResult?.Value as ToDoTask;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult?.StatusCode);

            Assert.NotNull(todoTask);
            Assert.Equal(id, todoTask?.Id);
            Assert.Equal(expectedTask, todoTask);
        }

        [Fact]
        public async Task GetToDoTask_WithNonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var id = 99;
            _mockDataProvider.Setup(m => m.GetToDoTaskAsync(id)).ReturnsAsync((ToDoTask?)null);

            // Act
            var result = await _controller.GetToDoTask(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetIncomingToDoTask_WithoutFilter_ReturnsOkObjectResultAndTodayTasks()
        {
            // Arrange
            var filter = IncomingTasksFilter.Today;
            var expectedTasks = new List<ToDoTask> { new ToDoTask { Id = 1, Title = "Today Task", Description = "Description" } };
            _mockDataProvider.Setup(m => m.GetIncomingToDoTasksAsync(filter)).ReturnsAsync(expectedTasks);

            // Act
            var result = await _controller.GetIncomingToDoTasks(filter);
            var okResult = result as OkObjectResult;
            var tasks = okResult?.Value as IEnumerable<ToDoTask>;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expectedTasks, tasks);
        }

        [Fact]
        public async Task GetIncomingToDoTask_WithThisWeekFilter_ReturnsOkObjectResultAndWithThisWeekTasks()
        {
            // Arrange
            var filter = IncomingTasksFilter.ThisWeek;
            var expectedTasks = new List<ToDoTask> { new ToDoTask { Id = 1, Title = "This Week Task", Description = "Description" } };
            _mockDataProvider.Setup(m => m.GetIncomingToDoTasksAsync(filter)).ReturnsAsync(expectedTasks);

            // Act
            var result = await _controller.GetIncomingToDoTasks(filter);
            var okResult = result as OkObjectResult;
            var tasks = okResult?.Value as IEnumerable<ToDoTask>;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expectedTasks, tasks);
        }

        [Fact]
        public async Task CreateToDoTask_ValidTask_ReturnsCreatedAtActionResultWithTask()
        {
            // Arrange
            var newTask = new ToDoTask { Id = 0, Title = "New Task", Description = "Description" };
            var createdTask = new ToDoTask { Id = 1, Title = "New Task", Description = "Description" };
            _mockDataProvider.Setup(m => m.CreateToDoTaskAsync(newTask)).ReturnsAsync(createdTask);

            // Act
            var result = await _controller.CreateToDoTask(newTask);
            var createdAtActionResult = result as CreatedAtActionResult;
            var task = createdAtActionResult?.Value as ToDoTask;

            // Assert
            Assert.NotNull(createdAtActionResult);
            Assert.Equal("GetToDoTask", createdAtActionResult.ActionName);
            Assert.Equal(createdTask, task);
        }

        [Fact]
        public async Task UpdateToDoTask_ValidTask_ReturnsOkObjectResultWithUpdatedTask()
        {
            // Arrange
            var existingTask = new ToDoTask { Id = 1, Title = "Existing Task", Description = "Description" };
            _mockDataProvider.Setup(m => m.UpdateToDoTaskAsync(existingTask)).ReturnsAsync(existingTask);

            // Act
            var result = await _controller.UpdateToDoTask(existingTask.Id, existingTask);
            var okResult = result as OkObjectResult;
            var updatedTask = okResult?.Value as ToDoTask;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(existingTask, updatedTask);
        }

        [Fact]
        public async Task UpdateToDoTask_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var task = new ToDoTask { Id = 1, Title = "Task", Description = "Description" };

            // Act
            var result = await _controller.UpdateToDoTask(2, task);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID in URL does not match ID in body.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateToDoTask_NonExistingTask_ReturnsNotFound()
        {
            // Arrange
            var task = new ToDoTask { Id = 1, Title = "Task", Description = "Description" };
            _mockDataProvider.Setup(m => m.UpdateToDoTaskAsync(task)).ReturnsAsync((ToDoTask?)null);

            // Act
            var result = await _controller.UpdateToDoTask(task.Id, task);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SetToDoTaskProgress_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var id = 1;
            var progress = 0.5M;
            var task = new ToDoTask() { Id = id, Title = "Task", Description = "Description", PercentComplete = progress };
            _mockDataProvider.Setup(m => m.SetToDoTaskProgressAsync(id, progress)).ReturnsAsync(task);

            // Act
            var result = await _controller.SetToDoTaskProgress(id, progress);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task SetToDoTaskProgress_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var id = 1;
            var progress = 0.5M;
            _mockDataProvider.Setup(m => m.SetToDoTaskProgressAsync(id, progress)).ReturnsAsync((ToDoTask?)null);

            // Act
            var result = await _controller.SetToDoTaskProgress(id, progress);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task MarkToDoTaskAsDone_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var id = 1;
            var task = new ToDoTask() { Id = id, Title = "Task", Description = "Description", IsDone = true };
            _mockDataProvider.Setup(m => m.MarkToDoTaskAsDoneAsync(id)).ReturnsAsync(task);

            // Act
            var result = await _controller.MarkToDoTaskAsDone(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task MarkToDoTaskAsDone_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var id = 1;
            _mockDataProvider.Setup(m => m.MarkToDoTaskAsDoneAsync(id)).ReturnsAsync((ToDoTask?)null);

            // Act
            var result = await _controller.MarkToDoTaskAsDone(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteToDoTask_ValidTask_ReturtnNoContent()
        {
            // Arrange
            var id = 1;
            _mockDataProvider.Setup(m => m.DeleteToDoTaskAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteToDoTask(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteToDoTask_NonExistingTask_ReturnsNotFound()
        {
            // Arrange
            var id = 1;
            _mockDataProvider.Setup(m => m.DeleteToDoTaskAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteToDoTask(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}