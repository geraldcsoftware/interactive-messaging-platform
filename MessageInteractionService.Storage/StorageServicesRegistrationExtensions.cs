using MessageInteractionService.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MessageInteractionService.Storage;

public static class StorageServicesRegistrationExtensions
{
    public static void AddMessageInteractionStorageServices(this IServiceCollection services,
                                  Action<DbContextOptionsBuilder> configureDbOptions)

    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<ISessionStore, SessionStore>();
        services.AddDbContext<MessagingDbContext>(configureDbOptions);
    }
}