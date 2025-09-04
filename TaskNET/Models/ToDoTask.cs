using System.ComponentModel.DataAnnotations.Schema;
using TaskNET.Interfaces;

namespace TaskNET.Models
{
    public class ToDoTask : IAuditable
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }

        public DateTime? ExpiryDate { get; set; }
        [Column(TypeName = "decimal(5, 4)")]
        public decimal PercentComplete { get; set; } = 0.0000M;
        public bool IsDone { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}