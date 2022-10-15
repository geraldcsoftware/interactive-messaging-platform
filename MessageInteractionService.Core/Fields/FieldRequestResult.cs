using System.Globalization;

namespace MessageInteractionService.Core.Fields;

public class FieldRequestResult
{
    public FieldRequestResult(string? rawValue, bool isValid, OutgoingMessage? userMessage)
    {
        RawValue = rawValue;
        HasValue = !string.IsNullOrEmpty(rawValue);
        IsValid = isValid;
        UserMessage = userMessage;
    }

    public string? RawValue { get; }
    public bool HasValue { get; }
    public bool IsValid { get; }
    public OutgoingMessage? UserMessage { get; }

    public  T GetValue<T>() where T: IParsable<T>
    {
        if (T.TryParse(RawValue, CultureInfo.InvariantCulture , out T value)) return value;

        throw FieldValueFormatException.New(RawValue, typeof(T));
    }
}

