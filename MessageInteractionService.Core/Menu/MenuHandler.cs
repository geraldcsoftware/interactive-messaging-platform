using System.Text;
using MessageInteractionService.Core.Handlers;
using MessageInteractionService.Core.Input;
using MessageInteractionService.Core.Sessions;

namespace MessageInteractionService.Core.Menu;

public class MenuHandler : HandlerBase
{
    private readonly IMenuProvider _menuProvider;
    private readonly IHandlerFactory _handlerFactory;

    public MenuHandler(ISession session,
                       IMenuProvider menuProvider,
                       IDateTimeProvider dateTimeProvider,
                       ISessionStore sessionStore,
                       IHandlerFactory handlerFactory) :
        base(dateTimeProvider, sessionStore)
    {
        Session = session;
        _menuProvider = menuProvider;
        _handlerFactory = handlerFactory;
    }

    public override ISession Session { get; }

    public override async Task<OutgoingMessage> Handle(IncomingMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        if (string.IsNullOrEmpty(message.Body))
            return InvalidInput();

        var input = ParseInput(message.Body, InputType.ItemPosition);
        if (input is not PositionInputValue { IsValid: true } selectedPositionInput)
        {
            return InvalidInput();
        }

        var hasMenuIdDefined = Session.Data.TryGetValue("CurrentMenuId", out var previousMenuId);
        if (!hasMenuIdDefined || string.IsNullOrEmpty(previousMenuId))
        {
            var rootMenu = await _menuProvider.GetRootMenuElement();
            var responseMsg = BuildMenuResponse(rootMenu);

            Session.Data["CurrentMenuId"] = rootMenu.Id;
            await UpdateSession();
            return responseMsg;
        }

        if (selectedPositionInput.IsNavigation)
        {
            return new OutgoingMessage
            {
                Body = "Navigation is not supported yet",
                Recipient = Session.Sender,
                SessionId = Session.Id,
                TimeSent = DateTimeProvider.UtcNow,
                TerminateSession = true
            };
        }

        var selectedMenu = await _menuProvider.GetChildMenu(previousMenuId,
                                                            selectedPositionInput.Position);

        if (selectedMenu == null)
        {
            return InvalidInput();
        }

        if (!string.IsNullOrEmpty(selectedMenu.HandlerName))
        {
            Session.Data.Add("Handler", selectedMenu.HandlerName);
            Session.Data["CurrentMenuId"] = selectedMenu.Id;
            await UpdateSession();

            var handlerInstance = await _handlerFactory.GetMessageHandler(Session);
            if (handlerInstance.GetType() != GetType())
            {
                return await handlerInstance.Handle(message);
            }

            throw new Exception($"Cannot have {handlerInstance.GetType().Name} as a handler type");
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

        var body = builder.ToString().Trim();
        return new OutgoingMessage
        {
            SessionId = Session.Id,
            TimeSent = DateTimeProvider.UtcNow,
            Recipient = Session.Sender,
            Body = body,
            TerminateSession = false
        };
    }
}