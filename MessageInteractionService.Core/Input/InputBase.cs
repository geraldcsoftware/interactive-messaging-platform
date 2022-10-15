namespace MessageInteractionService.Core.Input;

public abstract class InputBase
{
    public abstract InputType Type { get; }
    public abstract bool IsValid { get; }
    public string? RawValue { get; set; }
}