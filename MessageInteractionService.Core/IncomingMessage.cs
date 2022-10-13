namespace MessageInteractionService.Core;

public class IncomingMessage
{
    public required string Sender { get; set; }
    public required DateTimeOffset ReceivedTime { get; set; }
    public required string Body { get; set; }
    public required string SessionId { get; set; }
}