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
    }
}