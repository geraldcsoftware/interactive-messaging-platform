namespace MessageInteractionService.Core;

public interface ISessionFactory
{
    Task<ISession> GetOrCreateSession(IncomingMessage message);
    ISessionStore SessionStore { get; }
}

public interface IHandlerFactory
{
    Task<IMessageHandler> GetMessageHandler(ISession session);
}

public interface IMessageHandler
{
    ISession Session { get; }
    Task<OutgoingMessage> Handle(IncomingMessage message);
}