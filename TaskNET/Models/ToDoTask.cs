using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskNET.Interfaces;

namespace TaskNET.Models
{
    public class ToDoTask : IAuditable
    {
        public required int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MinLength(3, ErrorMessage = "Title must be at least 3 characters long.")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        public required string Description { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Range(0.0, 1.0, ErrorMessage = "PercentComplete must be between 0 and 1.")]
        [Column(TypeName = "decimal(5, 4)")]
        public decimal PercentComplete { get; set; } = 0.0000M;
        
        public bool IsDone { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}