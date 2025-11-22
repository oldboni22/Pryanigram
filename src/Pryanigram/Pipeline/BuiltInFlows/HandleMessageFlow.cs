using Pryanigram.Handler.Provider.Contract;

namespace Pryanigram.Pipeline.BuiltInFlows;

public sealed class HandleMessageFlow(IHandlerProvider provider) : IFlowEntry
{
    public async Task InvokeAsync(FlowContext context, FlowDelegate next)
    {
        var handler = provider.ConstructHandler(context);
        
        if (handler is not null)
        {
            try
            {
                await handler.HandleAsync(context);
                context.IsHandled = true;
            }
            finally
            {
                if (handler is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
        
        await next(context);
    }
}
