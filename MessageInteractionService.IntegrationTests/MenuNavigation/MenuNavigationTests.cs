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
}