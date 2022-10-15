namespace MessageInteractionService.Api.Endpoints.Models;

public class MessageResponse
{
    public string? To { get; set; }
    public DateTimeOffset Time { get; set; }
    public string? Content { get; set; }
    public bool Terminal { get; set; }
}