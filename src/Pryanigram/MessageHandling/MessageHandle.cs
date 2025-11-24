using Pryanigram.Pipeline;

namespace Pryanigram.MessageHandling;

public abstract class MessageHandle
{
    internal static readonly string FlowContextFieldName = nameof(FlowContext);
    
    public abstract Task HandleAsync();

    protected FlowContext FlowContext { get; private set; } = null!;

}
