using System.Text;
using System.Text.Json;
using Dapper;
using FluentAssertions;

namespace MessageInteractionService.IntegrationTests.EndpointTests;

[Collection("SharedMenu")]
public class MessageEndpointTests

{
    private readonly DataSetupFixture _fixture;

    public MessageEndpointTests(DataSetupFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task WhenNewUserSendsMessage_ShouldGetRootMenuContent()
    {
        // Arrange
        var client = _fixture.Factory.CreateClient();
        const string url = "api/messages";
        var body = $$"""
            {
                "body": "#",
                "sender": "testSender",
                "sent": "{{DateTimeOffset.UtcNow:o}}"
            }    
        """ ;
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

    [Fact]
    public async Task WhenNewUserSendsMessage_ShouldCreateSenderAnsStartSession()
    {
        // Arrange
        var client = _fixture.Factory.CreateClient();
        const string url = "api/messages";
        var body = $$"""
            {
                "body": "#",
                "sender": "testSender2",
                "sent": "{{DateTimeOffset.UtcNow:o}}"
            }    
        """ ;
        var content = new StringContent(body, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync(url, content);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        await using var sqlConnection = new Npgsql.NpgsqlConnection(_fixture.Factory.GetConnectionString());
        const string sendersQuery = """
            SELECT COUNT(*) FROM "Senders" WHERE "Key" = @sender 
            """;
            
        const string sessionQuery = """
            SELECT COUNT(*) FROM "Sessions" JOIN "Senders" 
            ON "Sessions"."SenderId" = "Senders"."Id"
             WHERE "Senders"."Key" = @sender 
            """;
        await sqlConnection.OpenAsync();
        var sendersCount = await sqlConnection.ExecuteScalarAsync<int>(sendersQuery, new { @sender = "testSender2" });
        var sessionsCount = await sqlConnection.ExecuteScalarAsync<int>(sessionQuery, new { @sender = "testSender2" });
        await sqlConnection.CloseAsync();

        sendersCount.Should().Be(1);
        sessionsCount.Should().Be(1);
    }
}