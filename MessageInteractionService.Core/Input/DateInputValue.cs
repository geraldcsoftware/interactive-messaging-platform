using System.Globalization;

namespace MessageInteractionService.Core.Input;

public class DateInputValue: InputBase
{
    private readonly DateTime _dateValue;
    public DateInputValue(string value)
    {
        var isValid = DateTime.TryParseExact(value.AsSpan(),
                                             "dd/MM/yyyy".AsSpan(),
                                             CultureInfo.CurrentCulture,
                                             DateTimeStyles.AdjustToUniversal,
                                             out var dateValue);
        IsValid = isValid;
        _dateValue = dateValue;
    }
    public override InputType Type => InputType.Date;
    public override bool IsValid { get; }

    public DateTime Value
    {
        get
        {
            if (IsValid) return _dateValue;
            throw new InvalidOperationException("Input was not a valid date");
        }
    }
}