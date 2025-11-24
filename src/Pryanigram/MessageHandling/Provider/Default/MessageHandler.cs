using System.Collections.ObjectModel;
using Pryanigram.MessageHandling.Provider.Contract;
using Pryanigram.Pipeline;

namespace Pryanigram.MessageHandling.Provider.Default;

public sealed class MessageHandler(ReadOnlyDictionary<string, Func<FlowContext, Task>> handlers) : IMessageHandler
{
    public async Task HandleMessage(FlowContext context)
    {
        if(!handlers.TryGetValue(context.Command!, out var handler))
        {
            return;
        }
        
        await handler.Invoke(context);
    }

    public bool HasCommand(string command)
    {
        return handlers.ContainsKey(command);
    }
}
