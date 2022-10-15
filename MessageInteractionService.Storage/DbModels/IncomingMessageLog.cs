namespace MessageInteractionService.Storage.DbModels;

public sealed class IncomingMessageLog : MessageLog
{
    public required string From { get; set; }
}