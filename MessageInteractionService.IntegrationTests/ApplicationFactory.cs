using MessageInteractionService.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MessageInteractionService.IntegrationTests;

public class ApplicationFactory : WebApplicationFactory<Api.Endpoints.ProcessIncomingMessageEndpoint>
{
}

public class InMemoryApplicationFactory : ApplicationFactory
{
    private readonly string _dbName;

    public InMemoryApplicationFactory()
    {
        _dbName = Guid.NewGuid().ToString();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            var dbOptionsDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(DbContextOptions));
            if (dbOptionsDescriptor == null) return;
            
            services.Remove(dbOptionsDescriptor);

            services.AddDbContext<MessagingDbContext>(options => options.UseInMemoryDatabase(_dbName));

        });
    }
}