using System.Text;

namespace MessageInteractionService.Core.Menu;

public class MenuHandler : HandlerBase
{
    private readonly IMenuProvider _menuProvider;

    public MenuHandler(ISession session,
                       IMenuProvider menuProvider,
                       IDateTimeProvider dateTimeProvider,
                       ISessionStore sessionStore) :
        base(dateTimeProvider, sessionStore)
    {
        Session = session;
        _menuProvider = menuProvider;
    }

    public override ISession Session { get; }

    public override async Task<OutgoingMessage> Handle(IncomingMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        if (string.IsNullOrEmpty(message.Body))
            return InvalidInput();

        var selectedPositionInput = ParseInput(message.Body, InputType.ItemPosition);
        if (selectedPositionInput.IsValid)
        {
            return InvalidInput();
        }

        var selectedMenu = await _menuProvider.GetChildMenu(Session.Data["CurrentMenuId"],
                                                            int.Parse(selectedPositionInput.Value));

        if (selectedMenu == null)
        {
            return InvalidInput();
        }

        var response = BuildMenuResponse(selectedMenu);

        Session.Data["CurrentMenuId"] = selectedMenu.Id;
        await UpdateSession();
        return response;
    }

    private OutgoingMessage BuildMenuResponse(MenuElement menu)
    {
        ArgumentNullException.ThrowIfNull(menu);

        var builder = new StringBuilder();
        if (!string.IsNullOrEmpty(menu.HeaderText))
            builder.AppendLine(menu.HeaderText);

        foreach (var menuOption in menu.Options.OrderBy(o => o.DisplayPosition))
        {
            builder.Append($"{menuOption.DisplayPosition}. ");
            builder.AppendLine(menuOption.OptionText);
        }

        var body = builder.ToString();
        return new OutgoingMessage
        {
            SessionId = Session.Id,
            TimeSent = DateTimeProvider.UtcNow,
            Recipient = Session.Sender,
            Body = body
        };
    }
}