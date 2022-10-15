namespace MessageInteractionService.Core;

public interface IMessageProcessor
{
    Task<OutgoingMessage> ProcessMessage(IncomingMessage message);
}