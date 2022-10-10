using AutoMapper;

namespace MessageInteractionService.Api.Mappings;

public class IncomingMessageProfile : Profile
{
    public IncomingMessageProfile()
    {
        CreateMap<Endpoints.Models.IncomingMessage, Core.IncomingMessage>()
           .ForMember(d => d.Body, o => o.MapFrom(src => src.Body))
           .ForMember(d => d.ReceivedTime, o => o.MapFrom(src => src.Sent ?? DateTimeOffset.UtcNow))
           .ForMember(d => d.SessionId, o => o.Ignore());
    }
}