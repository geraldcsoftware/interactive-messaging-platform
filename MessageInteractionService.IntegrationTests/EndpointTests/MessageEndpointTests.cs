using System.Text;
using System.Text.Json;
using Dapper;
using FluentAssertions;

namespace MessageInteractionService.IntegrationTests.EndpointTests;

public class MessageEndpointTests : IClassFixture<ApplicationFactory>
{
    private readonly ApplicationFactory _factory;

    public MessageEndpointTests(ApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task WhenNewUserSendsMessage_ShouldGetRootMenuContent()
    {
        // Arrange
        var connectionString = _factory.GetConnectionString();

        await using (var connection = new Npgsql.NpgsqlConnection(connectionString))
        {
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

        var client = _factory.CreateClient();
        const string url = "api/messages";
        var body = $$"""
            {
                "body": "#",
                "sender": "testSender",
                "sent": "{{ DateTimeOffset.UtcNow:o}}"
            }    
        """;
        var content = new StringContent(body, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync(url, content);

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseJson = JsonDocument.Parse(responseContent);
        const string expected = """
        Welcome to my test
        1. First option
        """;

        responseJson.RootElement.GetProperty("content")
                    .GetString()
                    .Should()
                    .BeEquivalentTo(expected);
    }
}