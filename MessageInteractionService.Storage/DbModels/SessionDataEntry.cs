namespace MessageInteractionService.Storage.DbModels;

public class SessionDataEntry
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string? Key { get; set; }
    public string? Value { get; set; }
}