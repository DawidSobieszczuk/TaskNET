using Microsoft.EntityFrameworkCore;
using TaskNET.Models;

namespace TaskNET.Data
{
    /// <summary>
    /// Application's DbContext, represents a session with the database
    /// </summary>
    /// <param name="options"></param>
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<ToDoTask> ToDoTasks { get; set; }
    }
}
