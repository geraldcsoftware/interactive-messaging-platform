using AutoMapper;
using MessageInteractionService.Core;

namespace MessageInteractionService.Api.Mappings;

public class OutgoingMessageProfile: Profile
{
    public OutgoingMessageProfile()
    {
        CreateMap<OutgoingMessage,
                Endpoints.Models.MessageResponse>()
           .ForMember(d => d.Content, o => o.MapFrom(s => s.Body))
           .ForMember(d => d.Time, o => o.MapFrom(s => s.TimeSent))
           .ForMember(d => d.To, o => o.MapFrom(s => s.Recipient))
           .ForMember(d => d.Terminal, o => o.MapFrom(s => s.TerminateSession));
    }
}