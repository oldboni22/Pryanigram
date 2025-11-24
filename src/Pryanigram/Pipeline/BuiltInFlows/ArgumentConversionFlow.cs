using Pryanigram.ArgumentsConversion.Converter.Contract;

namespace Pryanigram.Pipeline.BuiltInFlows;

public sealed class ArgumentConversionFlow(IArgumentConverter converter) : IFlowEntry
{
    public async Task InvokeAsync(FlowContext context, FlowDelegate next)
    {
        if (string.IsNullOrEmpty(context.Command) || string.IsNullOrEmpty(context.Arguments))
        {
            await next(context);
            return;
        }
        
        var converted = await converter.Convert(context);

        context.ConvertedArguments = converted;
        
        await next(context);
    }
}