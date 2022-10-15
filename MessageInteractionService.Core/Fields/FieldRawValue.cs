namespace MessageInteractionService.Core.Fields;

public record FieldRawValue
{
    public string? Value { get; set; }
    public bool ValidationResult { get; set; }
};