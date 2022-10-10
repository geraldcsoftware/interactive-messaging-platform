using MessageInteractionService.Core.Input;

namespace MessageInteractionService.Core;

public abstract class HandlerBase : IMessageHandler
{
    protected readonly IDateTimeProvider DateTimeProvider;
    protected readonly ISessionStore SessionStore;

    protected HandlerBase(IDateTimeProvider dateTimeProvider,
                          ISessionStore sessionStore)
    {
        DateTimeProvider = dateTimeProvider;
        SessionStore = sessionStore;
    }

    protected OutgoingMessage InvalidInput()
    {
        return new OutgoingMessage
        {
            SessionId = Session.Id,
            Recipient = Session.Sender,
            TimeSent = DateTimeProvider.UtcNow,
            Body = "Invalid input, please try again."
        };
    }

    public abstract ISession Session { get; }
    public abstract Task<OutgoingMessage> Handle(IncomingMessage message);

    protected static InputBase ParseInput(string input, InputType expectedType)
    {
        return expectedType switch
        {
            InputType.ItemPosition => new PositionInputValue(input.Trim()),
            InputType.Date         => new DateInputValue(input.Trim()),
            _                      => new RawTextInputValue(input.Trim())
        };
    }
    
    protected Task UpdateSession() => SessionStore.UpdateSession(Session);
}