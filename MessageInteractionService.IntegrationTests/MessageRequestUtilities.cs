using System.Text;
using System.Text.Json;

namespace MessageInteractionService.IntegrationTests;

public static class MessageRequestUtilities
{
    public static async Task<string?> SendRequest(this HttpClient httpClient, string content, string sender)
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
        var response = await httpClient.PostAsync(url, requestContent);

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new
                Exception($"Request resulted in a non-success response [{response.StatusCode}] - {responseContent}");

        var responseJson = JsonDocument.Parse(responseContent);
        return responseJson.RootElement.GetProperty("content").GetString();
    }
}