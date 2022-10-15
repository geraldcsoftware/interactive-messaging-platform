namespace MessageInteractionService.Storage.DbModels;

public sealed class OutgoingMessageLog : MessageLog
{
    public required string To { get; set; }
}