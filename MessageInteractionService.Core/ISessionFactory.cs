namespace MessageInteractionService.Core;

public interface ISessionFactory
{
    Task<ISession> GetOrCreateSession(IncomingMessage message);
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