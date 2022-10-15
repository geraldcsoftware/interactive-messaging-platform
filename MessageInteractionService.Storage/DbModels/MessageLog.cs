namespace MessageInteractionService.Storage.DbModels;

public abstract class MessageLog
{
    public required Guid Id { get; set; }
    public required Guid ParticipantId { get; set; }
    public required string Body { get; set; }
    public required Guid SessionId { get; set; }
    public required DateTimeOffset Time { get; set; }
}