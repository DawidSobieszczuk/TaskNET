using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using TaskNET.Data;
using Testcontainers.MySql; // Ensure this is the correct using for MySqlBuilder
using Xunit;

namespace TaskNET.Test.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<Program>
    {
        private readonly MySqlContainer _mySqlContainer;

        public CustomWebApplicationFactory()
        {
            // Set the environment variable for the test environment
            Environment.SetEnvironmentVariable("TEST_ENVIRONMENT", "false");

            _mySqlContainer = new MySqlBuilder()
                .WithImage("mysql:8.0")
                .WithUsername("testuser")
                .WithPassword("testpassword")
                .WithDatabase("testdb")
                .Build();

            _mySqlContainer.StartAsync().Wait();

            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
            {
                _mySqlContainer.StopAsync().Wait();
            };            
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
                services.RemoveAll<AppDbContext>(); 

                // Add DbContext for MySQL using Testcontainers connection string
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseMySql(_mySqlContainer.GetConnectionString(), ServerVersion.AutoDetect(_mySqlContainer.GetConnectionString()));
                });

                // Ensure the database is created and migrations are applied
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();
                db.Database.Migrate(); // Apply migrations
            });
        }
    }
}