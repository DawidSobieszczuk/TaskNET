using Microsoft.EntityFrameworkCore;
using TaskNET.Models;

namespace TaskNET.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<ToDoTask> ToDoTasks { get; set; }
    }
}
