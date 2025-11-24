using Pryanigram.Pipeline;

namespace Pryanigram.MessageHandling;

public abstract class ControllerBase
{
    internal static readonly string FlowContextFieldName = nameof(FlowContext);

    protected FlowContext FlowContext { get; private set; } = null!;
}
