using Dapper;
using FluentAssertions;

namespace MessageInteractionService.IntegrationTests.MenuNavigation;

[Collection("ExtendedMenuData")]
public class MenuNavigationTests
{
    private readonly ExtendedMenuTreeDataFixture _fixture;

    public MenuNavigationTests(ExtendedMenuTreeDataFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task WhenNavigatingToFirstOption_ShouldShowFirstOptionChildMenus()
    {
        //Arrange
        var client = _fixture.Factory.CreateClient();
        var sender = new Bogus.Faker().Person.Email;

        // Act
        var menuResponse = await client.SendRequest("#", sender);
        var firstItemResponse = await client.SendRequest("1", sender);
        
        // Assert
        const string expectedResponse1 = """ 
        Welcome to my tests
        1. Debug Test
        2. Run Test
        """;
        const string expectedResponse2 = """
        Select environment
        1. Development
        2. Staging
        3. Production
        """;
        menuResponse.Should().BeEquivalentTo(expectedResponse1);
        firstItemResponse.Should().BeEquivalentTo(expectedResponse2);
    }
    
    [Fact]
    public async Task WhenUserInteractsWithSystem_ShouldLogIncomingAndOutgoingMessages()
    {
        //Arrange
        var client = _fixture.Factory.CreateClient();
        var sender = new Bogus.Faker().Person.Email;

        // Act
        _ = await client.SendRequest("#", sender);
        _ = await client.SendRequest("1", sender);
        
        // Assert
        const string expectedResponse1 = """ 
        Welcome to my tests
        1. Debug Test
        2. Run Test
        """;
        const string expectedResponse2 = """
        Select environment
        1. Development
        2. Staging
        3. Production
        """;

        const string messagesQuery = """
                SELECT "Body", "MessageDirection" FROM "MessageLogs" WHERE  "ParticipantId" = (
                SELECT "Id" FROM "Senders" WHERE "Key" = @sender) 
                ORDER BY "Time" 
                """;
       await using var connection = new Npgsql.NpgsqlConnection(_fixture.Factory.GetConnectionString());
       await connection.OpenAsync();

       var messages = await connection.QueryAsync<(string Body, string MessageDirection)>(messagesQuery, new { sender });

       messages.Should().HaveCount(4);
       messages.Where(m => m.MessageDirection == "OUTGOING").Should().HaveCount(2);
       messages.Where(m => m.MessageDirection == "INCOMING").Should().HaveCount(2);
       messages.Where(m => m.Body == expectedResponse1).Should().HaveCount(1);
       messages.Where(m => m.Body == expectedResponse2).Should().HaveCount(1);
       
    }
}