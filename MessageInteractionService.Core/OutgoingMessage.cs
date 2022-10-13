namespace MessageInteractionService.Core;

public class OutgoingMessage
{
    public required string Recipient { get; set; }
    public required string Body { get; set; }
    public required DateTimeOffset TimeSent { get; set; }
    public required string SessionId { get; set; }
}