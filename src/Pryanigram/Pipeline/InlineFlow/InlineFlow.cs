namespace Pryanigram.Pipeline.InlineFlow;

internal class InlineFlow(Func<FlowContext, FlowDelegate, Task> flow) : IFlowEntry
{
    public async Task InvokeAsync(FlowContext context, FlowDelegate next)
    {
        await flow(context, next);
    }
}
