using Pryanigram.MessageHandling.Provider.Contract;

namespace Pryanigram.Pipeline.BuiltInFlows;

public sealed class HandleMessageFlow(IMessageHandler handler) : IFlowEntry
{
    public async Task InvokeAsync(FlowContext context, FlowDelegate next)
    {
        if (!string.IsNullOrEmpty(context.Command))
        {
            await handler.HandleMessage(context);    
        }
        
        await next(context);
    }
}
