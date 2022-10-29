using MessageInteractionService.Core.Sessions;

namespace MessageInteractionService.Core;

public interface IMessageLogger
{
    Task LogMessage(IncomingMessage message, ISession session);
    Task LogMessage(OutgoingMessage message, ISession session);
}