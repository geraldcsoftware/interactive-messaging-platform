using DotNet.Testcontainers.Builders;
using MessageInteractionService.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;

namespace MessageInteractionService.IntegrationTests;

public class ApplicationFactory : WebApplicationFactory<Api.Endpoints.ProcessIncomingMessageEndpoint>,
                                  IAsyncLifetime
{
    private readonly PostgreSqlContainer _testContainer;

    public ApplicationFactory()
    {
        var waitStrategy = Wait.ForUnixContainer()
                               .UntilOperationIsSucceeded(CheckDatabaseAvailability, 10);

        _testContainer = new PostgreSqlBuilder()
                        .WithDatabase("MessagingApp.Tests")
                        .WithUsername("postgres")
                        .WithPassword("postgres")
                        .WithWaitStrategy(waitStrategy)
                        .Build();
    }

    private bool CheckDatabaseAvailability()
    {
        try
        {
            using var connection = new NpgsqlConnection(_testContainer.GetConnectionString());
            connection.Open();

            using var command = new NpgsqlCommand
            {
                Connection = connection,
                CommandText = "SELECT 1"
            };
            var result = command.ExecuteReader();
            return result.Read();
        }
        catch (Exception)
        {
            return false;
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureAppConfiguration(configuration =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:StorageConnection", _testContainer.GetConnectionString() }
            });
        });
    }

    public string GetConnectionString()
    {
        return _testContainer.GetConnectionString();
    }

    public async Task InitializeAsync()
    {
        await _testContainer.StartAsync();
        using var scope = Server.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MessagingDbContext>();
        if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
            await dbContext.Database.MigrateAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return _testContainer.DisposeAsync().AsTask();
    }
}