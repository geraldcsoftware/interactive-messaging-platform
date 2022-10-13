using MessageInteractionService.Core;

namespace MessageInteractionService.Api.Services;

public class MessageProcessor : IMessageProcessor
{
    private readonly ISessionFactory _sessionFactory;
    private readonly IHandlerFactory _handlerFactory;
    private readonly IMessageLogger _messageLogger;

    public MessageProcessor(ISessionFactory sessionFactory,
                            IHandlerFactory handlerFactory,
                            IMessageLogger messageLogger)
    {
        _sessionFactory = sessionFactory;
        _handlerFactory = handlerFactory;
        _messageLogger = messageLogger;
    }

    public async Task<OutgoingMessage> ProcessMessage(IncomingMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        var session = await _sessionFactory.GetOrCreateSession(message);

        await _messageLogger.LogMessage(message, session);
        
        var handler = await _handlerFactory.GetMessageHandler(session);
        var responseMessage = await handler.Handle(message);

        await _messageLogger.LogMessage(responseMessage, session);
        return responseMessage;
    }
}