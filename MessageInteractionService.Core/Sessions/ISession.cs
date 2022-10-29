namespace MessageInteractionService.Core.Sessions;

public interface ISession
{
    string Id { get; }
    string Sender { get; }
    DateTimeOffset StartTime { get; }
    IDictionary<string, string> Data { get; }
}