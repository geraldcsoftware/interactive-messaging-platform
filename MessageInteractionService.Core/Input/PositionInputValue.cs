namespace MessageInteractionService.Core.Input;

public class PositionInputValue : InputBase
{
    private readonly int _position;
    private readonly bool _isPosition;
    private readonly bool _isNav;

    public PositionInputValue(string value)
    {
        RawValue = value;
        if (int.TryParse(value, out var position))
        {
            _position = position;
            _isPosition = true;
            IsValid = true;
        }
        else if (value is "n" or "*" or "#")
        {
            _isNav = true;
            _isPosition = false;
            IsValid = true;
        }
        else
        {
            IsValid = false;
        }
    }

    public override InputType Type => InputType.ItemPosition;

    public int Position
    {
        get
        {
            if (_isPosition) return _position;
            throw new InvalidOperationException("Not a position value");
        }
    }

    public override bool IsValid { get; }
    public bool IsPosition => _isPosition;
    public bool IsNavigation => _isNav;
}