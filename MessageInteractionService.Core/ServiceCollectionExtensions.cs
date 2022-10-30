using MessageInteractionService.Core.Handlers;
using MessageInteractionService.Core.Sessions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MessageInteractionService.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageInteractionCoreServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IMessageProcessor, MessageProcessor>()
                .AddScoped<IHandlerFactory, HandlerFactory>()
                .AddScoped<ISessionFactory, SessionFactory>()
                .TryAddTransient<IDateTimeProvider, Clock>();

        return services;
    }
}