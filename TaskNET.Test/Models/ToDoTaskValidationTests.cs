using System.ComponentModel.DataAnnotations;
using TaskNET.Models;
using Xunit;

namespace TaskNET.Test.Models
{
    public class ToDoTaskValidationTests
    {
        private static bool ValidateModel(ToDoTask model)
        {
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(model, validationContext, validationResults, true);
        }

        [Fact]
        public void Title_Required_ShouldBeInvalid()
        {
            var task = new ToDoTask { Id = 1, Title = "", Description = "Test Description", PercentComplete = 0.5M };
            Assert.False(ValidateModel(task));
        }

        [Fact]
        public void Title_MinLength_ShouldBeInvalid()
        {
            var task = new ToDoTask { Id = 1, Title = "ab", Description = "Test Description", PercentComplete = 0.5M };
            Assert.False(ValidateModel(task));
        }

        [Fact]
        public void Title_MaxLength_ShouldBeInvalid()
        {
            var task = new ToDoTask { Id = 1, Title = new string('a', 101), Description = "Test Description", PercentComplete = 0.5M };
            Assert.False(ValidateModel(task));
        }

        [Fact]
        public void Title_Valid_ShouldBeValid()
        {
            var task = new ToDoTask { Id = 1, Title = "Valid Title", Description = "Test Description", PercentComplete = 0.5M };
            Assert.True(ValidateModel(task));
        }

        [Fact]
        public void Description_Required_ShouldBeInvalid()
        {
            var task = new ToDoTask { Id = 1, Title = "Valid Title", Description = "", PercentComplete = 0.5M };
            Assert.False(ValidateModel(task));
        }

        [Fact]
        public void Description_StringLength_ShouldBeInvalid()
        {
            var task = new ToDoTask { Id = 1, Title = "Valid Title", Description = new string('a', 251), PercentComplete = 0.5M };
            Assert.False(ValidateModel(task));
        }

        [Fact]
        public void Description_Valid_ShouldBeValid()
        {
            var task = new ToDoTask { Id = 1, Title = "Valid Title", Description = "Valid Description", PercentComplete = 0.5M };
            Assert.True(ValidateModel(task));
        }

        [Fact]
        public void PercentComplete_Range_TooLow_ShouldBeInvalid()
        {
            var task = new ToDoTask { Id = 1, Title = "Valid Title", Description = "Valid Description", PercentComplete = -0.1M };
            Assert.False(ValidateModel(task));
        }

        [Fact]
        public void PercentComplete_Range_TooHigh_ShouldBeInvalid()
        {
            var task = new ToDoTask { Id = 1, Title = "Valid Title", Description = "Valid Description", PercentComplete = 1.1M };
            Assert.False(ValidateModel(task));
        }

        [Fact]
        public void PercentComplete_Range_Valid_ShouldBeValid()
        {
            var task = new ToDoTask { Id = 1, Title = "Valid Title", Description = "Valid Description", PercentComplete = 0.5M };
            Assert.True(ValidateModel(task));
        }

        [Fact]
        public void ExpiryDate_Valid_ShouldBeValid()
        { 
            // Arrange
            var task = new ToDoTask { Id = 1, Title = "Valid Title", Description = "Valid Description", PercentComplete = 0.5M, ExpiryDate = DateTime.Now.AddDays(1) };

            // Act & Assert
            Assert.True(ValidateModel(task));
        }

                [Fact]
        public void ExpiryDate_Null_ShouldBeValid()
        {
            // Arrange
            var task = new ToDoTask { Id = 1, Title = "Valid Title", Description = "Valid Description", ExpiryDate = null };

            // Act & Assert
            Assert.True(ValidateModel(task));
        }

        [Fact]
        public void IsDone_DefaultValue_ShouldBeFalse()
        {
            // Arrange
            var task = new ToDoTask { Id = 1, Title = "Valid Title", Description = "Valid Description" };

            // Act & Assert
            Assert.False(task.IsDone);
            Assert.True(ValidateModel(task));
        }

                [Fact]
        public void CreatedAt_DefaultValue_ShouldBeSet()
        {
            // Arrange
            var task = new ToDoTask { Id = 1, Title = "Test", Description = "Desc" };
            
            // Assert
            Assert.NotEqual(default(DateTime), task.CreatedAt);
            Assert.True(ValidateModel(task)); // Ensure it's still valid after default value
        }

        [Fact]
        public void UpdatedAt_NullDefault_ShouldBeNull()
        {
            // Arrange
            var task = new ToDoTask { Id = 1, Title = "Valid Title", Description = "Valid Description" };

            // Act & Assert
            Assert.Null(task.UpdatedAt);
            Assert.True(ValidateModel(task));
        }
    }
}