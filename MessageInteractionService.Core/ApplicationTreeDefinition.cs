namespace MessageInteractionService.Core;

public class ApplicationTreeDefinition
{
    public required Guid Id { get; init; }
    public required string Header { get; init; }
    public required bool IsActive { get; set; }
}