namespace MessageInteractionService.Storage.DbModels;

public abstract class MessageLog
{
    public Guid Id { get; set; }
    public Guid ParticipantId { get; set; }
    public string Body { get; set; }
    public Guid SessionId { get; set; }
    public DateTimeOffset Time { get; set; }
}

public sealed class OutgoingMessageLog : MessageLog
{
    public string To { get; set; }
}

public sealed class IncomingMessageLog : MessageLog
{
    public string From { get; set; }
}