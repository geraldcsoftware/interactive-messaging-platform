using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using MessageInteractionService.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace MessageInteractionService.IntegrationTests;

public class ApplicationFactory : WebApplicationFactory<Api.Endpoints.ProcessIncomingMessageEndpoint>,
                                  IAsyncLifetime
{
    private readonly PostgreSqlTestcontainer _testContainer;

    public ApplicationFactory()
    {
        var dbConfiguration = new PostgreSqlTestcontainerConfiguration
        {
            Database = "MessagingApp.Tests",
            Username = "postgres",
            Password = "postgres"
        };
        var waitStrategy = Wait.ForUnixContainer()
                               .UntilOperationIsSucceeded(CheckDatabaseAvailability, 10);

        _testContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                        .WithDatabase(dbConfiguration)
                        .WithWaitStrategy(waitStrategy)
                        .Build();
    }

    private bool CheckDatabaseAvailability()
    {
        try
        {
            using var connection = new NpgsqlConnection(_testContainer.ConnectionString);
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
                { "ConnectionStrings:StorageConnection", _testContainer.ConnectionString }
            });
        });
    }

    public string GetConnectionString()
    {
        return _testContainer.ConnectionString;
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