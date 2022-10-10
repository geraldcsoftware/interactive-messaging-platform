namespace MessageInteractionService.Api.Endpoints.Models;

public class IncomingMessage
{
    public string? Sender { get; set; }
    public string? Body { get; set; }
    public DateTimeOffset? Sent { get; set; }
}