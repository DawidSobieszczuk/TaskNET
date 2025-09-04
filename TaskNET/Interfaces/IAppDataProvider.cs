using TaskNET.Models;

namespace TaskNET.Interfaces
{
    public enum IncomingTasksFilter
    {
        Today,
        Tomorrow,
        ThisWeek
    }

    public interface IAppDataProvider
    {
        Task<IEnumerable<ToDoTask>> GetToDoTasksAsync();
        Task<ToDoTask?> GetToDoTaskAsync(int id);
        Task<IEnumerable<ToDoTask>> GetIncomingToDoTasksAsync(IncomingTasksFilter incomingTasksFilter = IncomingTasksFilter.Today);

        Task<ToDoTask> CreateToDoTaskAsync(ToDoTask toDoTask);
        Task<ToDoTask?> UpdateToDoTaskAsync(ToDoTask toDoTask);

        Task<ToDoTask?> SetToDoTaskProgressAsync(int id, decimal percent);
        Task<ToDoTask?> MarkToDoTaskAsDoneAsync(int id);
        Task<bool> DeleteToDoTaskAsync(int id);

    }
}