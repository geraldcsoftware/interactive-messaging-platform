namespace MessageInteractionService.Core.Fields;

public class FieldValueFormatException: Exception
{
    private FieldValueFormatException(string message):base(message)
    { }
    
    public string RawValue { get; private set; }
    public Type ExpectedType { get; private set; }
    public static FieldValueFormatException New(string rawValue, Type expectedType)
    {
        return new FieldValueFormatException($"Field could not be converted to type '{expectedType.Name}`")
        {
            RawValue = rawValue,
            ExpectedType = expectedType
        };
    }
}