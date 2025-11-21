namespace Pryanigram.Pipeline;

public delegate Task FlowDelegate(FlowContext context);

public abstract class FlowEntry
{
    public abstract Task InvokeAsync(FlowContext context, FlowDelegate next);
}