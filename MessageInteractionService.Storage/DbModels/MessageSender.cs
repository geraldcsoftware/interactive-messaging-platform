namespace MessageInteractionService.Storage.DbModels;

public class MessageSender
{
    public Guid Id { get; set; }
    public string? Key { get; set; }
    public bool Enabled { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? Updated { get; set; }
}