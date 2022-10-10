using System.Globalization;

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

    public Input ParseInput(string input, InputType expectedType)
    {
        switch (expectedType)
        {
            case InputType.ItemPosition:
                return ParsePositionInput(input);
            case InputType.Date:
                return ParseDateInput(input);
            default: return ParseTextInput(input);
        }
    }

    private Input ParsePositionInput(string input)
    {
        var trimmed = input.Trim();
        if (trimmed == "*") // next
        {
            return new Input
            {
                IsValid = true,
                Value = trimmed,
                Type = InputType.ItemPosition
            };
        }

        var isValid = int.TryParse(trimmed, out var position);
        return new Input
        {
            IsValid = isValid,
            Value = trimmed, // todo: make Input generic, so that Value can be of different types. to avoid re-parsing
            Type = InputType.ItemPosition
        };
    }

    private Input ParseDateInput(string input)
    {
        var isValid = DateTime.TryParseExact(input.AsSpan(),
                                             "dd/MM/yyyy".AsSpan(),
                                             CultureInfo.CurrentCulture,
                                             DateTimeStyles.AdjustToUniversal,
                                             out _);
        return new() { Value = input, IsValid = isValid, Type = InputType.Text };
    }

    private Input ParseTextInput(string input) => new() { Value = input, IsValid = true, Type = InputType.Text };

    protected Task UpdateSession() => SessionStore.UpdateSession(Session);
}