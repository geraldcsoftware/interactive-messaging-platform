namespace MessageInteractionService.Core.Input;

public class RawTextInputValue : InputBase
{
    public RawTextInputValue(string value)
    {
        Value = value;
    }

    public override InputType Type => InputType.Text;
    public override bool IsValid => true;

    public string Value { get; }
}