using MessageInteractionService.Core;
using MessageInteractionService.Core.Menu;
using MessageInteractionService.Core.Sessions;
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
        services.AddScoped<IMenuProvider, DbMenuProvider>();
        services.AddScoped<IMessageLogger, DbMessageLogger>();
        services.AddDbContext<MessagingDbContext>(configureDbOptions);
    }
}