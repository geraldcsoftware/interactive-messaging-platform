using FastEndpoints;
using MessageInteractionService.Api.Endpoints.Models;
using MessageInteractionService.Api.Services;
using IMapper = AutoMapper.IMapper;

namespace MessageInteractionService.Api.Endpoints;

public class ProcessIncomingMessageEndpoint : Endpoint<IncomingMessage, MessageResponse>
{
    private readonly IMessageProcessor _messageProcessor;
    private readonly IMapper _mapper;

    public ProcessIncomingMessageEndpoint(IMessageProcessor messageProcessor,
                                          IMapper mapper)
    {
        _messageProcessor = messageProcessor;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Post("api/messages");
        AllowAnonymous();
    }

    public override async Task HandleAsync(IncomingMessage req, CancellationToken ct)
    {
        var message = _mapper.Map<MessageInteractionService.Core.IncomingMessage>(req);
        Logger.LogInformation("Beginning processing of message {@Message}", message);
        var result = await _messageProcessor.ProcessMessage(message);
        Logger.LogInformation("Message processing result {@Result}", result);
        var response = _mapper.Map<MessageResponse>(result);

        await SendOkAsync(response, ct);
    }
}