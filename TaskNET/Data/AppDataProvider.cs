using Microsoft.EntityFrameworkCore;
using TaskNET.Interfaces;
using TaskNET.Models;

namespace TaskNET.Data
{
    public class AppDataProvider : IAppDataProvider
    {
        private readonly AppDbContext _context;

        public AppDataProvider(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ToDoTask> CreateToDoTaskAsync(ToDoTask toDoTask)
        {
            toDoTask.CreatedAt = DateTime.UtcNow;
            _context.ToDoTasks.Add(toDoTask);
            await _context.SaveChangesAsync();
            return toDoTask;
        }

        public async Task<bool> DeleteToDoTaskAsync(int id)
        {
            var task = await _context.ToDoTasks.FindAsync(id);
            if (task == null)
            {
                return false;
            }
            _context.ToDoTasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ToDoTask>> GetIncomingToDoTasksAsync(IncomingTasksFilter incomingTasksFilter = IncomingTasksFilter.Today)
        {
            var today = DateTime.UtcNow.Date;
            IQueryable<ToDoTask> query = incomingTasksFilter switch
            {
                IncomingTasksFilter.Today => _context.ToDoTasks.Where(t => t.ExpiryDate != null && t.ExpiryDate.Value.Date == today),
                IncomingTasksFilter.Tomorrow => _context.ToDoTasks.Where(t => t.ExpiryDate != null && t.ExpiryDate.Value.Date == today.AddDays(1)),
                IncomingTasksFilter.ThisWeek => _context.ToDoTasks.Where(t => t.ExpiryDate != null && t.ExpiryDate.Value.Date >= today && t.ExpiryDate.Value.Date <= today.AddDays(7)),
                _ => Enumerable.Empty<ToDoTask>().AsQueryable(),
            };
            return await query.Where(t => !t.IsDone).ToListAsync();
        }

        public async Task<ToDoTask?> GetToDoTaskAsync(int id)
        {
            return await _context.ToDoTasks.FindAsync(id);
        }

        public async Task<IEnumerable<ToDoTask>> GetToDoTasksAsync()
        {
            return await _context.ToDoTasks.ToListAsync();
        }

        public async Task<ToDoTask?> MarkToDoTaskAsDoneAsync(int id)
        {
            var task = await _context.ToDoTasks.FindAsync(id);
            if (task != null)
            {
                task.IsDone = true;
                task.PercentComplete = 1.0M;
                task.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return task;
        }

        public async Task<ToDoTask?> SetToDoTaskProgressAsync(int id, decimal percent)
        {
            var task = await _context.ToDoTasks.FindAsync(id);
            if (task != null)
            {
                task.PercentComplete = percent;
                task.UpdatedAt = DateTime.UtcNow;
                if (percent >= 1.0M)
                {
                    task.IsDone = true;
                }
                await _context.SaveChangesAsync();
            }
            return task;
        }

        public async Task<ToDoTask?> UpdateToDoTaskAsync(ToDoTask toDoTask)
        {
            var existingTask = await _context.ToDoTasks.FindAsync(toDoTask.Id);
            if (existingTask == null)
            {
                return null;
            }

            existingTask.Title = toDoTask.Title;
            existingTask.Description = toDoTask.Description;
            existingTask.ExpiryDate = toDoTask.ExpiryDate;
            existingTask.PercentComplete = toDoTask.PercentComplete;
            existingTask.IsDone = toDoTask.IsDone;
            existingTask.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return existingTask;
        }
    }
}