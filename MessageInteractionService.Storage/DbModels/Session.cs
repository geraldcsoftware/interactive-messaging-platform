namespace MessageInteractionService.Storage.DbModels;

public class Session
{
    public Guid Id { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset? Terminated { get; set; }
    public Guid SenderId { get; set; }
    public MessageSender? Sender { get; set; }
    public ICollection<SessionDataEntry> DataEntries { get; set; } = new List<SessionDataEntry>();
}