
using TaskNET.Interfaces;
using TaskNET.Models;

namespace TaskNET.Data
{
    public class InMemoryDataProvider : IAppDataProvider
    {
        private readonly List<ToDoTask> _tasks;

        public InMemoryDataProvider()
        {
            _tasks = [];
        }

        public Task<ToDoTask> CreateToDoTaskAsync(ToDoTask toDoTask)
        {
            var newId = _tasks.Count != 0 ? _tasks.Max(t => t.Id) + 1 : 1;
            toDoTask.Id = newId;
            toDoTask.CreatedAt = DateTime.UtcNow;
            _tasks.Add(toDoTask);
            return Task.FromResult(toDoTask);
        }

        public Task<bool> DeleteToDoTaskAsync(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return Task.FromResult(false);
            }
            _tasks.Remove(task);
            return Task.FromResult(true);
        }

        public Task<IEnumerable<ToDoTask>> GetIncomingToDoTasksAsync(IncomingTasksFilter incomingTasksFilter = IncomingTasksFilter.Today)
        {
            var today = DateTime.UtcNow.Date;
            IEnumerable<ToDoTask> result = incomingTasksFilter switch
            {
                IncomingTasksFilter.Today => _tasks.Where(t => t.ExpiryDate?.Date == today),
                IncomingTasksFilter.Tomorrow => _tasks.Where(t => t.ExpiryDate?.Date == today.AddDays(1)),
                IncomingTasksFilter.ThisWeek => _tasks.Where(t => t.ExpiryDate?.Date >= today && t.ExpiryDate?.Date <= today.AddDays(7 - (int)today.DayOfWeek)),
                _ => Enumerable.Empty<ToDoTask>(),
            };
            return Task.FromResult(result.Where(t => !t.IsDone));
        }

        public Task<ToDoTask?> GetToDoTaskAsync(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            return Task.FromResult(task);
        }

        public Task<IEnumerable<ToDoTask>> GetToDoTasksAsync()
        {
            return Task.FromResult(_tasks.AsEnumerable());
        }

        public Task<ToDoTask?> MarkToDoTaskAsDoneAsync(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                task.IsDone = true;
                task.PercentComplete = 1.0M;
                task.UpdatedAt = DateTime.UtcNow;
            }
            return Task.FromResult(task);
        }

        public Task<ToDoTask?> SetToDoTaskProgressAsync(int id, decimal percent)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                task.PercentComplete = percent;
                task.UpdatedAt = DateTime.UtcNow;
                if (percent >= 1.0M)
                {
                    task.IsDone = true;
                }
            }
            return Task.FromResult(task);
        }

        public Task<ToDoTask?> UpdateToDoTaskAsync(ToDoTask toDoTask)
        {
            var existingTask = _tasks.FirstOrDefault(t => t.Id == toDoTask.Id);
            if (existingTask == null)
            {
                return Task.FromResult<ToDoTask?>(null);
            }

            existingTask.Title = toDoTask.Title;
            existingTask.Description = toDoTask.Description;
            existingTask.ExpiryDate = toDoTask.ExpiryDate;
            existingTask.PercentComplete = toDoTask.PercentComplete;
            existingTask.IsDone = toDoTask.IsDone;
            existingTask.UpdatedAt = DateTime.UtcNow;

            return Task.FromResult<ToDoTask?>(existingTask);
        }
    }
}