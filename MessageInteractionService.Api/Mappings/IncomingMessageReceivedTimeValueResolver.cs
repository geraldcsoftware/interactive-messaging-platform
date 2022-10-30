using MessageInteractionService.Core;

namespace MessageInteractionService.Api.Mappings;

using AutoMapper;

public class IncomingMessageReceivedTimeValueResolver :
    IValueResolver<Endpoints.Models.IncomingMessage, IncomingMessage, DateTimeOffset>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public IncomingMessageReceivedTimeValueResolver(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public DateTimeOffset Resolve(Endpoints.Models.IncomingMessage source,
                                  IncomingMessage destination,
                                  DateTimeOffset destMember,
                                  ResolutionContext context)
        => source.Sent ?? _dateTimeProvider.UtcNow;
}