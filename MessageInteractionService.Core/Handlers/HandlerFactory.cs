using System.Reflection;
using MessageInteractionService.Core.Fields;
using MessageInteractionService.Core.Menu;
using MessageInteractionService.Core.Sessions;
using Microsoft.Extensions.DependencyInjection;

namespace MessageInteractionService.Core.Handlers;

public class HandlerFactory : IHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public HandlerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<IMessageHandler> GetMessageHandler(ISession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        var hasHandlerDefined = session.Data.TryGetValue("Handler", out var currentHandler)
                                && !string.IsNullOrEmpty(currentHandler);

        return Task.FromResult(hasHandlerDefined ? GetHandler(currentHandler, session) : GetMenuHandler(session));
    }

    private IMessageHandler GetMenuHandler(ISession session)
    {
        var handler = ActivatorUtilities.CreateInstance<MenuHandler>(_serviceProvider, session);
        return handler;
    }

    private IMessageHandler GetHandler(string? handlerName, ISession session)
    {
        ArgumentException.ThrowIfNullOrEmpty(handlerName);
        var type = ResolveFromKnownType(handlerName) ?? Assembly.GetAssembly(GetType())?.GetType(handlerName);
        
        if (type == null)
            throw new ArgumentException($"'{handlerName}' is an unknown type.", nameof(handlerName));

        if (!type.IsAssignableTo(typeof(IMessageHandler)))
            throw new InvalidOperationException($"Type '{handlerName}' is not assignable to IMessageHandler.");

        ISessionFieldStore sessionFieldStore = ActivatorUtilities.CreateInstance<SessionFieldStore>(_serviceProvider, session);
        var handler = ActivatorUtilities.CreateInstance(_serviceProvider, type, session, sessionFieldStore);
        return (IMessageHandler)handler;
    }

    private static Type? ResolveFromKnownType(string typeName)
    {
        return typeName switch
        {
            nameof(KycHandler) => typeof(KycHandler),
            _                  => null
        };
    }
}