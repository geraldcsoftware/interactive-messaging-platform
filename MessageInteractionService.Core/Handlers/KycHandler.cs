using MessageInteractionService.Core.Fields;

namespace MessageInteractionService.Core.Handlers;

public class KycHandler : HandlerBase
{
    private readonly ISessionFieldStore _sessionFieldStore;

    public KycHandler(ISession session,
                      IDateTimeProvider dateTimeProvider,
                      ISessionStore sessionStore,
                      ISessionFieldStore sessionFieldStore) :
        base(dateTimeProvider, sessionStore)
    {
        _sessionFieldStore = sessionFieldStore;
        Session = session;
    }

    public sealed override ISession Session { get; }

    public override async Task<OutgoingMessage> Handle(IncomingMessage message)
    {
        var firstNameField = await _sessionFieldStore.FieldRequest("FirstName")
                                                     .WithPromptMessage("Enter your first name")
                                                     .WithInput(message.Body)
                                                     .WithValidation(CommonValidators.IsNameValid,
                                                                     "Please enter a valid first name")
                                                     .RequestField();

        if (firstNameField is not { IsValid: true, HasValue: true })
            return firstNameField.UserMessage!;

        var lastNameField = await _sessionFieldStore.FieldRequest("LastName")
                                                    .WithPromptMessage("Enter your last name")
                                                    .WithInput(message.Body)
                                                    .WithValidation(CommonValidators.IsNameValid,
                                                                    "Please enter a valid last name")
                                                    .RequestField();

        if (lastNameField is not { IsValid: true, HasValue: true })
            return lastNameField.UserMessage!;

        await Task.Delay(500);
        return new()
        {
            Body = $$"""
            Thank you {{firstNameField.RawValue}} {{lastNameField.RawValue}},
            Have a nice day!
            """,
            Recipient = Session.Sender,
            SessionId = Session.Id,
            TimeSent = DateTimeProvider.UtcNow,
            TerminateSession = true
        };
    }
}