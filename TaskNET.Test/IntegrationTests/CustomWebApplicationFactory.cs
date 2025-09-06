using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskNET.Data;
using Testcontainers.MySql;

namespace TaskNET.Test.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<Program>
    {
        private readonly MySqlContainer _mySqlContainer;

        public CustomWebApplicationFactory()
        {
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

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseMySql(_mySqlContainer.GetConnectionString(), ServerVersion.AutoDetect(_mySqlContainer.GetConnectionString()));
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            });
        }
    }
}