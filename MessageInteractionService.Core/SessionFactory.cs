namespace MessageInteractionService.Core;

public class SessionFactory : ISessionFactory
{
    private readonly ISessionStore _sessionStore;

    public SessionFactory(ISessionStore sessionStore)
    {
        _sessionStore = sessionStore;
    }

    public async Task<ISession> GetOrCreateSession(IncomingMessage message)
    {
        var session = await _sessionStore.GetSenderCurrentActiveSession(message.Sender);
        return session ?? await _sessionStore.CreateSession(message);
    }
}