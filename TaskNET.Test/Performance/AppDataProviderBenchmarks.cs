using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using TaskNET.Data;
using TaskNET.Interfaces;
using TaskNET.Models;

namespace TaskNET.Test.Performance;

[MemoryDiagnoser]
public class AppDataProviderBenchmarks
{
    private IAppDataProvider _appDataProvider = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "PerfTests")
            .Options;
        var dbContext = new AppDbContext(options);
        dbContext.Database.EnsureCreated();

        _appDataProvider = new AppDataProvider(dbContext);

        // Seed the database with some data
        for (int i = 0; i < 100; i++)
        {
            _appDataProvider.CreateToDoTaskAsync(new ToDoTask
            {
                Id = 0, // Id will be set by the database
                Title = $"Test Task {i}",
                Description = "Performance test description"
            }).Wait();
        }
    }

    [Benchmark]
    public async Task GetToDoTasksAsync_Benchmark()
    {
        await _appDataProvider.GetToDoTasksAsync();
    }

    [Benchmark]
    public async Task CreateToDoTaskAsync_Benchmark()
    {
        var newTask = new ToDoTask
        {
            Id = 0,
            Title = "New Perf Task",
            Description = "A new task for the benchmark"
        };
        await _appDataProvider.CreateToDoTaskAsync(newTask);
    }
}
