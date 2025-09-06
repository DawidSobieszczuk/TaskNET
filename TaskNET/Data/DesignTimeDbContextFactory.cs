using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TaskNET.Data
{
    /// <summary>
    /// Potrzebne do migracji w czasie projektowania
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseMySql("Server=mysql;Database=tasknet;User=tasknet;Password=TFmVkA3ZRE45", ServerVersion.Create(8,0,12, Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}