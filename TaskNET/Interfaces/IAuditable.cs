namespace TaskNET.Interfaces
{
    // Interface for tracking entity creation and update times
    public interface IAuditable
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}