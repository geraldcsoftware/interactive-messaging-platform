namespace MessageInteractionService.Core;

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

        try
        {
            var session = await _sessionFactory.GetOrCreateSession(message);

            await _messageLogger.LogMessage(message, session);

            var handler = await _handlerFactory.GetMessageHandler(session);
            var responseMessage = await handler.Handle(message);
            if (responseMessage.TerminateSession)
            {
                await _sessionFactory.SessionStore.TerminateSession(session);
            }

            await _messageLogger.LogMessage(responseMessage, session);
            return responseMessage;
        }
        catch (Exception)
        {
            var session = await _sessionFactory.SessionStore.GetSenderCurrentActiveSession(message.Sender);
            if (session is { })
                await _sessionFactory.SessionStore.TerminateSession(session);

            return new()
            {
                Body = "There was an error processing your request.",
                Recipient = message.Sender,
                SessionId = session?.Id,
                TerminateSession = true,
                TimeSent = DateTimeOffset.UtcNow
            };
        }
    }
}