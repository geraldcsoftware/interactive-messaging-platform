namespace MessageInteractionService.Core;

public interface ISession
{
    string Id { get; }
    string Sender { get; }
    DateTimeOffset StartTime { get; }
    IDictionary<string, string> Data { get; }
}