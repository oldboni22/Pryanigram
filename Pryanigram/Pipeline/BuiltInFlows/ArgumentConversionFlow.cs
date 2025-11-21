using Pryanigram.ArgumentsConversion.Converter.Contract;

namespace Pryanigram.Pipeline.BuiltInFlows;

public sealed class ArgumentConversionFlow(IArgumentConverter converter) : FlowEntry
{
    public override async Task InvokeAsync(FlowContext context, FlowDelegate next)
    {
        var converted = await converter.Convert(context);

        context.ConvertedArguments = converted;
        
        await next(context);
    }
}