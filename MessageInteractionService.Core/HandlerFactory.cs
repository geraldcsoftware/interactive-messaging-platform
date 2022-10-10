﻿using System.Reflection;
using MessageInteractionService.Core.Menu;
using Microsoft.Extensions.DependencyInjection;

namespace MessageInteractionService.Core;

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

        return Task.FromResult(hasHandlerDefined ? GetHandler(currentHandler) : GetMenuHandler(session));
    }

    private IMessageHandler GetMenuHandler(ISession session)
    {
        var handler = ActivatorUtilities.CreateInstance<MenuHandler>(_serviceProvider, session);
        return handler;
    }

    private IMessageHandler GetHandler(string? handlerName)
    {
        ArgumentException.ThrowIfNullOrEmpty(handlerName);
        var type = Assembly.GetAssembly(GetType())?.GetType(handlerName);

        if (type == null)
            throw new ArgumentException($"'{handlerName}' is an unknown type.", nameof(handlerName));

        if (type.IsAssignableTo(typeof(IMessageHandler)))
            throw new InvalidOperationException($"Type '{handlerName}' is not assignable to IMessageHandler.");

        var handler = ActivatorUtilities.CreateInstance(_serviceProvider, type);
        return (IMessageHandler)handler;
    }
}