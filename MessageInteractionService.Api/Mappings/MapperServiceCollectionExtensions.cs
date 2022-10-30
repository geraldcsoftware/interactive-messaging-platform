using MessageInteractionService.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MessageInteractionService.Api.Mappings;

public static class MapperServiceCollectionExtensions
{
    public static void AddMapper(this IServiceCollection services)
    {
        services.TryAddTransient<IDateTimeProvider, Clock>();
        services.AddTransient<IncomingMessageReceivedTimeValueResolver>();
        services.AddAutoMapper(config =>
        {
            config.AddProfile<IncomingMessageProfile>();
            config.AddProfile<OutgoingMessageProfile>();
        });
    }
}