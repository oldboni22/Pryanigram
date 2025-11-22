namespace Pryanigram.Pipeline;

public delegate Task FlowDelegate(FlowContext context);

public interface IFlowEntry
{
    public abstract Task InvokeAsync(FlowContext context, FlowDelegate next);
}