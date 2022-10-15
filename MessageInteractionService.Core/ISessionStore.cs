namespace MessageInteractionService.Core;

public interface ISessionStore
{
    Task<ISession?> GetSenderCurrentActiveSession(string sender);
    Task<ISession> CreateSession(IncomingMessage message);
    Task<ISession> UpdateSession(ISession session);
    Task TerminateSession(ISession session);
}