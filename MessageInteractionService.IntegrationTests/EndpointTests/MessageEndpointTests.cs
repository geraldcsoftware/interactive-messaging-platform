using System.Text;
using FluentAssertions;

namespace MessageInteractionService.IntegrationTests.EndpointTests;

public class MessageEndpointTests : IClassFixture<InMemoryApplicationFactory>
{
    private readonly ApplicationFactory _factory;

    public MessageEndpointTests(InMemoryApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Test()
    {
        // Arrange
        var client = _factory.CreateClient();
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
        
    }
}