using System.Text;
using System.Text.Json;

namespace MessageInteractionService.Console;

public class MessagingApiClient
{
    private readonly HttpClient _httpClient = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5199", UriKind.Absolute)
    };
    
    public async Task<(string?, bool)> SendRequest(string content, string sender)
    {
        const string url = "api/messages";
        var body = $$"""
            {
                "body": "{{ content}}",
                "sender": "{{ sender}}",
                "sent": "{{DateTimeOffset.UtcNow:o}}"
            }    
        """ ;
        var requestContent = new StringContent(body, Encoding.UTF8, "application/json");

        // Act
        var response = await _httpClient.PostAsync(url, requestContent);

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new
                Exception($"Request resulted in a non-success response [{response.StatusCode}] - {responseContent}");

        var responseJson = JsonDocument.Parse(responseContent);
        var outputContent = responseJson.RootElement.GetProperty("content").GetString();
        var terminate = responseJson.RootElement.GetProperty("terminal").GetBoolean();

        return (outputContent, terminate);
    }
}