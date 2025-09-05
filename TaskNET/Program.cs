using Microsoft.EntityFrameworkCore;
using TaskNET.Data;
using TaskNET.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TasksNETDatabase"));

// Add services to the container.

builder.Services.AddScoped<IAppDataProvider, AppDataProvider>();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }