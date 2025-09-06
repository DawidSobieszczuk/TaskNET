using Microsoft.EntityFrameworkCore;
using TaskNET.Data;
using TaskNET.Models;
using TaskNET.Interfaces;

namespace TaskNET.Test.Data
{
    // Unit tests for AppDataProvider
    // They don't load into a real database, they use InMemoryDatabase
    public class AppDataProviderTests
    {
        private static AppDbContext GetDbContext(string dbName = "Tests")
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var context = new AppDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task CreateToDoTaskAsync_AddsNewTaskAndAssignsId()
        {
            // Arrange
            var dbContext = GetDbContext();
            var provider = new AppDataProvider(dbContext);
            var newTask = new ToDoTask { Id = 0, Title = "Test Task", Description = "Description for test" };

            // Act
            var createdTask = await provider.CreateToDoTaskAsync(newTask);

            // Assert
            Assert.NotNull(createdTask);
            Assert.True(createdTask.Id > 0);
            Assert.Equal(newTask.Title, createdTask.Title);
            Assert.Equal(newTask.Description, createdTask.Description);
            Assert.NotEqual(default(DateTime), createdTask.CreatedAt);

            var allTasks = await provider.GetToDoTasksAsync();
            Assert.Contains(createdTask, allTasks);
            Assert.Single(allTasks);
        }

        [Fact]
        public async Task CreateToDoTaskAsync_AssignsUniqueIds()
        {
            // Arrange
            var dbContext = GetDbContext();
            var provider = new AppDataProvider(dbContext);
            var task1 = new ToDoTask { Id = 0, Title = "Task 1", Description = "Desc 1" };
            var task2 = new ToDoTask { Id = 0, Title = "Task 2", Description = "Desc 2" };

            // Act
            var createdTask1 = await provider.CreateToDoTaskAsync(task1);
            var createdTask2 = await provider.CreateToDoTaskAsync(task2);

            // Assert
            Assert.NotEqual(createdTask1.Id, createdTask2.Id);
        }

        [Fact]
        public async Task GetToDoTaskAsync_ReturnsTaskWhenExists()
        {
            // Arrange
            var dbContext = GetDbContext();
            var provider = new AppDataProvider(dbContext);
            var newTask = new ToDoTask { Id = 0, Title = "Get Task", Description = "Description" };
            var createdTask = await provider.CreateToDoTaskAsync(newTask);

            // Act
            var fetchedTask = await provider.GetToDoTaskAsync(createdTask.Id);

            // Assert
            Assert.NotNull(fetchedTask);
            Assert.Equal(createdTask, fetchedTask);
        }

        [Fact]
        public async Task GetToDoTaskAsync_ReturnsNullWhenNotExists()
        {
            // Arrange
            var dbContext = GetDbContext();
            var provider = new AppDataProvider(dbContext);

            // Act
            var fetchedTask = await provider.GetToDoTaskAsync(999); // Non-existent ID

            // Assert
            Assert.Null(fetchedTask);
        }

        [Fact]
        public async Task GetToDoTasksAsync_ReturnsAllTasks()
        {
            // Arrange
            var dbContext = GetDbContext();
            var provider = new AppDataProvider(dbContext);
            await provider.CreateToDoTaskAsync(new ToDoTask { Id = 0, Title = "Task 1", Description = "Desc 1" });
            await provider.CreateToDoTaskAsync(new ToDoTask { Id = 0, Title = "Task 2", Description = "Desc 2" });

            // Act
            var allTasks = await provider.GetToDoTasksAsync();

            // Assert
            Assert.NotNull(allTasks);
            Assert.Equal(2, allTasks.Count());
        }

        [Fact]
        public async Task DeleteToDoTaskAsync_RemovesTaskWhenExists()
        {
            // Arrange
            var dbContext = GetDbContext();
            var provider = new AppDataProvider(dbContext);
            var newTask = new ToDoTask { Id = 0, Title = "Delete Task", Description = "Description" };
            var createdTask = await provider.CreateToDoTaskAsync(newTask);

            // Act
            var success = await provider.DeleteToDoTaskAsync(createdTask.Id);

            // Assert
            Assert.True(success);
            var fetchedTask = await provider.GetToDoTaskAsync(createdTask.Id);
            Assert.Null(fetchedTask);
        }

        [Fact]
        public async Task DeleteToDoTaskAsync_ReturnsFalseWhenNotExists()
        {
            // Arrange
            var dbContext = GetDbContext();
            var provider = new AppDataProvider(dbContext);

            // Act
            var success = await provider.DeleteToDoTaskAsync(999); // Non-existent ID

            // Assert
            Assert.False(success);
        }

        [Fact]
        public async Task UpdateToDoTaskAsync_UpdatesExistingTask()
        {
            // Arrange
            var dbContext = GetDbContext();
            var provider = new AppDataProvider(dbContext);
            var originalTask = new ToDoTask { Id = 0, Title = "Original", Description = "Original Desc" };
            var createdTask = await provider.CreateToDoTaskAsync(originalTask);

            var updatedTaskData = new ToDoTask
            {
                Id = createdTask.Id,
                Title = "Updated Title",
                Description = "Updated Description",
                ExpiryDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = await provider.UpdateToDoTaskAsync(updatedTaskData);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedTaskData.Title, result.Title);
            Assert.Equal(updatedTaskData.Description, result.Description);
            Assert.Equal(updatedTaskData.ExpiryDate, result.ExpiryDate);
            Assert.NotEqual(default(DateTime), result.UpdatedAt);

            var fetchedTask = await provider.GetToDoTaskAsync(createdTask.Id);
            Assert.NotNull(fetchedTask);
            Assert.Equal(updatedTaskData.Title, fetchedTask.Title);
        }

        [Fact]
        public async Task UpdateToDoTaskAsync_ReturnsNullWhenTaskNotExists()
        {
            // Arrange
            var dbContext = GetDbContext();
            var provider = new AppDataProvider(dbContext);
            var nonExistentTask = new ToDoTask { Id = 999, Title = "Non Existent", Description = "Desc" };

            // Act
            var result = await provider.UpdateToDoTaskAsync(nonExistentTask);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(0.5)]
        [InlineData(1.0)]
        public async Task SetToDoTaskProgressAsync_UpdatesPercentComplete(decimal percent)
        {
            // Arrange
            var dbContext = GetDbContext();
            var provider = new AppDataProvider(dbContext);
            var newTask = new ToDoTask { Id = 0, Title = "Progress Task", Description = "Desc" };
            var createdTask = await provider.CreateToDoTaskAsync(newTask);

            // Act
            var result = await provider.SetToDoTaskProgressAsync(createdTask.Id, percent);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(percent, result.PercentComplete);
            Assert.NotEqual(default(DateTime), result.UpdatedAt);
            if (percent >= 1.0M)
            {
                Assert.True(result.IsDone);
            }
            else
            {
                Assert.False(result.IsDone);
            }
        }

        [Fact]
        public async Task SetToDoTaskProgressAsync_ReturnsNullWhenTaskNotExists()
        {
            // Arrange
            var dbContext = GetDbContext();
            var provider = new AppDataProvider(dbContext);

            // Act
            var result = await provider.SetToDoTaskProgressAsync(999, 0.5M);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task MarkToDoTaskAsDoneAsync_SetsIsDoneAndPercentComplete()
        {
            // Arrange
            var dbContext = GetDbContext();
            var provider = new AppDataProvider(dbContext);
            var newTask = new ToDoTask { Id = 0, Title = "Done Task", Description = "Desc", PercentComplete = 0.2M };
            var createdTask = await provider.CreateToDoTaskAsync(newTask);

            // Act
            var result = await provider.MarkToDoTaskAsDoneAsync(createdTask.Id);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsDone);
            Assert.Equal(1.0M, result.PercentComplete);
            Assert.NotEqual(default(DateTime), result.UpdatedAt);
        }

        [Fact]
        public async Task MarkToDoTaskAsDoneAsync_ReturnsNullWhenTaskNotExists()
        {
            // Arrange
            var dbContext = GetDbContext();
            var provider = new AppDataProvider(dbContext);

            // Act
            var result = await provider.MarkToDoTaskAsDoneAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetIncomingToDoTasksAsync_ReturnsTodayTasks()
        {
            // Arrange
            var dbContext = GetDbContext();
            var provider = new AppDataProvider(dbContext);
            await provider.CreateToDoTaskAsync(new ToDoTask { Id = 0, Title = "Today Task", Description = "Desc", ExpiryDate = DateTime.UtcNow.Date.AddHours(10) });
            await provider.CreateToDoTaskAsync(new ToDoTask { Id = 0, Title = "Tomorrow Task", Description = "Desc", ExpiryDate = DateTime.UtcNow.Date.AddDays(1).AddHours(10) });

            // Act
            var incomingTasks = await provider.GetIncomingToDoTasksAsync(IncomingTasksFilter.Today);

            // Assert
            Assert.Single(incomingTasks);
            Assert.Contains(incomingTasks, t => t.Title == "Today Task");
        }

        [Fact]
        public async Task GetIncomingToDoTasksAsync_ReturnsTomorrowTasks()
        {
            // Arrange
            var dbContext = GetDbContext();
            var provider = new AppDataProvider(dbContext);
            await provider.CreateToDoTaskAsync(new ToDoTask { Id = 0, Title = "Today Task", Description = "Desc", ExpiryDate = DateTime.UtcNow.Date.AddHours(10) });
            await provider.CreateToDoTaskAsync(new ToDoTask { Id = 0, Title = "Tomorrow Task", Description = "Desc", ExpiryDate = DateTime.UtcNow.Date.AddDays(1).AddHours(10) });

            // Act
            var incomingTasks = await provider.GetIncomingToDoTasksAsync(IncomingTasksFilter.Tomorrow);

            // Assert
            Assert.Single(incomingTasks);
            Assert.Contains(incomingTasks, t => t.Title == "Tomorrow Task");
        }

        [Fact]
        public async Task GetIncomingToDoTasksAsync_ReturnsThisWeekTasks()
        {
            // Arrange
            var dbContext = GetDbContext();
            var provider = new AppDataProvider(dbContext);
            // Task for today
            await provider.CreateToDoTaskAsync(new ToDoTask { Id = 0, Title = "This Week Task", Description = "Desc", ExpiryDate = DateTime.UtcNow.Date.AddHours(1) });
            // Task for next week (should not be included)
            await provider.CreateToDoTaskAsync(new ToDoTask { Id = 0, Title = "Next Week Task", Description = "Desc", ExpiryDate = DateTime.UtcNow.Date.AddDays(8).AddHours(1) });

            // Act
            var incomingTasks = await provider.GetIncomingToDoTasksAsync(IncomingTasksFilter.ThisWeek);

            // Assert
            Assert.Single(incomingTasks);
            Assert.Contains(incomingTasks, t => t.Title == "This Week Task");
            Assert.DoesNotContain(incomingTasks, t => t.Title == "Next Week Task");
        }

        [Fact]
        public async Task GetIncomingToDoTasksAsync_ExcludesDoneTasks()
        {
            // Arrange
            var dbContext = GetDbContext();
            var provider = new AppDataProvider(dbContext);
            var doneTask = new ToDoTask { Id = 0, Title = "Done Task", Description = "Desc", ExpiryDate = DateTime.UtcNow.Date.AddHours(1), IsDone = true };
            var incomingTask = new ToDoTask { Id = 0, Title = "Incoming Task", Description = "Desc", ExpiryDate = DateTime.UtcNow.Date.AddHours(1), IsDone = false };
            await provider.CreateToDoTaskAsync(doneTask);
            await provider.CreateToDoTaskAsync(incomingTask);

            // Act
            var incomingTasks = await provider.GetIncomingToDoTasksAsync(IncomingTasksFilter.Today);

            // Assert
            Assert.Single(incomingTasks);
            Assert.Contains(incomingTasks, t => t.Title == "Incoming Task");
            Assert.DoesNotContain(incomingTasks, t => t.Title == "Done Task");
        }
    }
}