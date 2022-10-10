namespace MessageInteractionService.Core;

public class IncomingMessage
{
    public required string Sender { get; set; }
    public required DateTimeOffset ReceivedTime { get; set; }
    public required string Body { get; set; }
    public required string SessionId { get; set; }
}

public class OutgoingMessage
{
    public required string Recipient { get; set; }
    public required string Body { get; set; }
    public required DateTimeOffset TimeSent { get; set; }
    public required string SessionId { get; set; }
}

public class Session
{
    public required string Id { get; set; }
    public required DateTimeOffset StartTime { get; set; }
    public required string Sender { get; set; }
    public required string LastMessageId { get; set; }
    public required string SessionData { get; set; }
}