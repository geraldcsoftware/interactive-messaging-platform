using MessageInteractionService.Core;

namespace MessageInteractionService.Api.Services;

public interface IMessageProcessor
{
    Task<OutgoingMessage> ProcessMessage(IncomingMessage message);
}