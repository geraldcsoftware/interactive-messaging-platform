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

       var messages = (await connection.QueryAsync<(string Body, string MessageDirection)>(messagesQuery, new { sender })).ToList();

       messages.Should().HaveCount(4);
       messages.Where(m => m.MessageDirection == "OUTGOING").Should().HaveCount(2);
       messages.Where(m => m.MessageDirection == "INCOMING").Should().HaveCount(2);
       messages.Where(m => m.Body == expectedResponse1).Should().HaveCount(1);
       messages.Where(m => m.Body == expectedResponse2).Should().HaveCount(1);
       
    }
    
    [Fact]
    public async Task WhenNavigatingToChildHandler_ShouldInvokeHandler()
    {
        //Arrange
        var client = _fixture.Factory.CreateClient();
        var sender = new Bogus.Faker().Person.Email;

        // Act
        var menuResponse = await client.SendRequest("#", sender);
        var firstItemResponse = await client.SendRequest("2", sender);
        var firstNamePromptResponse = await client.SendRequest("3", sender);
        var lastNamePromptResponse = await client.SendRequest("Gerald", sender);
        var summaryResponse = await client.SendRequest("Chifanzwa", sender);
        
        // Assert
        const string expectedResponse1 = """ 
        Welcome to my tests
        1. Debug Test
        2. Run Test
        """;
        const string expectedResponse2 = """
        Choose speed
        1. Run Once
        2. Run continuous
        3. Run and break
        """;
        const string expectedResponse3 = """
        Enter your first name
        """;
        const string expectedResponse4 = """
        Enter your last name
        """;
        const string expectedResponse5 = """
            Thank you Gerald Chifanzwa,
            Have a nice day!
            """;
                     
        menuResponse.Should().BeEquivalentTo(expectedResponse1);
        firstItemResponse.Should().BeEquivalentTo(expectedResponse2);
        firstNamePromptResponse.Should().BeEquivalentTo(expectedResponse3);
        lastNamePromptResponse.Should().BeEquivalentTo(expectedResponse4);
        summaryResponse.Should().BeEquivalentTo(expectedResponse5);
    }

}