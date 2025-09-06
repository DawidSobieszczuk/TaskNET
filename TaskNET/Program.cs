using Microsoft.EntityFrameworkCore;
using TaskNET.Data;
using TaskNET.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// When the application has a configured connection to a MySQL database, it connects to it, otherwise it uses InMemoryDatabase
// In this project, docker-compose.yml sets environment variables for the connection to the MySQL database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString != null)
{    
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TasksNETDatabase"));
}

builder.Services.AddScoped<IAppDataProvider, AppDataProvider>();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();
app.MapControllers();


// Automatic database migration when the application uses MySQL, not needed with InMemoryDatabase
if (connectionString != null)
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}

app.Run();

// This is needed for integration tests where TProgram is used as a type parameter
public partial class Program { }