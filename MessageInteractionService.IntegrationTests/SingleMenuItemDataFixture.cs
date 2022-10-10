using Dapper;

namespace MessageInteractionService.IntegrationTests;

public class SingleMenuItemDataFixture : IAsyncLifetime
{
    private readonly ApplicationFactory _factory;

    public SingleMenuItemDataFixture()
    {
        _factory = new ApplicationFactory();
    }

    public ApplicationFactory Factory => _factory;

    private async Task InitializeMenu()
    {
        var connectionString = _factory.GetConnectionString();

        await using var connection = new Npgsql.NpgsqlConnection(connectionString);
        var rootMenu = new
        {
            Id = Guid.NewGuid(),
            ParentMenuId = (Guid?)null,
            DisplayText = "Welcome to my test",
            OptionText = (string?)null,
            DisplayOrder = -1,
            HandlerName = (string?)null
        };
        var childMenu = new
        {
            Id = Guid.NewGuid(),
            ParentMenuId = rootMenu.Id,
            DisplayText = "First option",
            OptionText = "First option",
            DisplayOrder = 1,
            HandlerName = (string?)null
        };
        await connection.OpenAsync();
        await connection.ExecuteAsync(DbCommands.InsertMenuCommand, rootMenu);
        await connection.ExecuteAsync(DbCommands.InsertMenuCommand, childMenu);
    }

    async Task IAsyncLifetime.InitializeAsync()
    {
        await _factory.InitializeAsync();
        await InitializeMenu();
    }

    public async Task DisposeAsync()
    {
        await ((IAsyncLifetime)_factory).DisposeAsync();
    }
}