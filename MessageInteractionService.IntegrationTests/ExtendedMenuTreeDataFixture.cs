using Dapper;

namespace MessageInteractionService.IntegrationTests;

public class ExtendedMenuTreeDataFixture : IAsyncLifetime
{
    private readonly ApplicationFactory _factory;

    public ExtendedMenuTreeDataFixture()
    {
        _factory = new();
    }

    public ApplicationFactory Factory => _factory;

    private async Task InitializeMenu()
    {
        var connectionString = _factory.GetConnectionString();

        await using var connection = new Npgsql.NpgsqlConnection(connectionString);
        
        await connection.OpenAsync();
        await connection.ExecuteAsync(DbCommands.InsertMenuCommand, MenuArguments());
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

    private static InsertMenuArgument[] MenuArguments()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var id3 = Guid.NewGuid();
        var id4 = Guid.NewGuid();
        var id5 = Guid.NewGuid();
        var id6 = Guid.NewGuid();
        var id7 = Guid.NewGuid();
        var id8 = Guid.NewGuid();
        var id9 = Guid.NewGuid();
        return new InsertMenuArgument[]
        {
            new()
            {
                Id = id1, DisplayText = "Welcome to my tests", OptionText = string.Empty, DisplayOrder = -1,
                HandlerName = null, ParentMenuId = null
            },
            new()
            {
                Id = id2, DisplayText = "Select environment", OptionText = "Debug Test", DisplayOrder = 1,
                HandlerName = null, ParentMenuId = id1
            },
            new()
            {
                Id = id3, DisplayText = "Choose speed", OptionText = "Run Test", DisplayOrder = 2, HandlerName = null,
                ParentMenuId = id1
            },
            new()
            {
                Id = id4, DisplayText = "Environment 1", OptionText = "Development", DisplayOrder = 1,
                HandlerName = null, ParentMenuId = id2
            },
            new()
            {
                Id = id5, DisplayText = "Environment 2", OptionText = "Staging", DisplayOrder = 2, HandlerName = null,
                ParentMenuId = id2
            },
            new()
            {
                Id = id6, DisplayText = "Environment 3", OptionText = "Production", DisplayOrder = 3,
                HandlerName = null, ParentMenuId = id2
            },

            new()
            {
                Id = id7, DisplayText = "Context 1", OptionText = "Run Once", DisplayOrder = 1, HandlerName = null,
                ParentMenuId = id3
            },
            new()
            {
                Id = id8, DisplayText = "Context 2", OptionText = "Run continuous", DisplayOrder = 2,
                HandlerName = null, ParentMenuId = id3
            },
            new()
            {
                Id = id9, DisplayText = "Context 3", OptionText = "Run and break", DisplayOrder = 3, HandlerName = null,
                ParentMenuId = id3
            }
        };
    }


    private class InsertMenuArgument
    {
        public required Guid Id { get; set; }
        public required Guid? ParentMenuId { get; set; }
        public required string? DisplayText { get; set; }
        public required string? OptionText { get; set; }
        public required int DisplayOrder { get; set; }
        public required string? HandlerName { get; set; }
    }
}