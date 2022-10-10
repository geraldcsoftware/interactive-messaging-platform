using MessageInteractionService.Core;
using MessageInteractionService.Core.Menu;
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
        services.AddDbContext<MessagingDbContext>(configureDbOptions);
    }
}