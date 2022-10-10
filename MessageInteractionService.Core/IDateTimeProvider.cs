namespace MessageInteractionService.Core;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}

public class Clock: IDateTimeProvider
{
    public DateTimeOffset UtcNow => global::System.DateTimeOffset.UtcNow;
}