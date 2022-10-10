namespace MessageInteractionService.Core;

public class Input
{
    public InputType Type { get; set; }
    public bool IsValid { get; set; }
    public string Value { get; set; }
}