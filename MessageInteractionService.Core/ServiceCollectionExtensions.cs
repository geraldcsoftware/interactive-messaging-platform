using Microsoft.Extensions.DependencyInjection;

namespace MessageInteractionService.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageInteractionCoreServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IMessageProcessor, MessageProcessor>()
                .AddScoped<IHandlerFactory, HandlerFactory>()
                .AddScoped<ISessionFactory, SessionFactory>()
                .AddTransient<IDateTimeProvider, Clock>();

        return services;
    }
}