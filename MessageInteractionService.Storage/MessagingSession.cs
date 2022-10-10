using MessageInteractionService.Core;

namespace MessageInteractionService.Storage;

public class MessagingSession : ISession
{
    public MessagingSession(string id, 
                            string sender, 
                            DateTimeOffset startTime, 
                            IDictionary<string, string> data)
    {
        Id = id;
        Sender = sender;
        StartTime = startTime;
        Data = data;
    }

    public string Id { get; }
    public string Sender { get; }
    public DateTimeOffset StartTime { get; }
    public IDictionary<string, string> Data { get; }
}