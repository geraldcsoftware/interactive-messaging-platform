using MessageInteractionService.Core;

namespace MessageInteractionService.Api.Services;

public class MessageProcessor : IMessageProcessor
{
    private readonly ISessionFactory _sessionFactory;
    private readonly IHandlerFactory _handlerFactory;

    public MessageProcessor(ISessionFactory sessionFactory,
                            IHandlerFactory handlerFactory)
    {
        _sessionFactory = sessionFactory;
        _handlerFactory = handlerFactory;
    }

    public async Task<OutgoingMessage> ProcessMessage(IncomingMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        var session = await _sessionFactory.GetOrCreateSession(message);
        var handler = await _handlerFactory.GetMessageHandler(session);

        var result = await handler.Handle(message);
        return result;
    }
}