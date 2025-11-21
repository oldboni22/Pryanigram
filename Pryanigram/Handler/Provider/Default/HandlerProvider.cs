using System.Collections.ObjectModel;
using Pryanigram.Handler.Provider.Contract;
using Pryanigram.Pipeline;

namespace Pryanigram.Handler.Provider.Default;

public sealed class HandlerProvider : IHandlerProvider
{
    internal HandlerProvider(ReadOnlyDictionary<string, Func<IServiceProvider, TelegramMessageHandler>> handlerSources)
    {
        _handlerSources = handlerSources;
    }

    private readonly ReadOnlyDictionary<string, Func<IServiceProvider, TelegramMessageHandler>> _handlerSources;
    
    public TelegramMessageHandler? ConstructHandler(FlowContext context)
    {
        if (string.IsNullOrEmpty(context.Command))
        {
            return null;
        }
        
        if (!_handlerSources.TryGetValue(context.Command!, out var source))
        {
            return null;
        }
        
        return source.Invoke(context.ServiceProvider);
    }

    public bool HasHandler(string command)
    {
        return _handlerSources.ContainsKey(command);
    }
}
