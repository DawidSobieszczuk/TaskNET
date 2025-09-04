using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TaskNET.Data;
using TaskNET.Interfaces;

namespace TaskNET.Test.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Replace the IAppDataProvider with an in-memory one for testing
                var dataProviderDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAppDataProvider));

                if (dataProviderDescriptor != null)
                {
                    services.Remove(dataProviderDescriptor);
                }

                services.AddSingleton<IAppDataProvider, InMemoryDataProvider>();
            });
        }
    }
}