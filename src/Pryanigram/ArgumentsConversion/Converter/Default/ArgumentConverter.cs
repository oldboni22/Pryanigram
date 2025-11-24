using System.Collections.ObjectModel;
using Pryanigram.ArgumentsConversion.Converter.Contract;
using Pryanigram.Exceptions;
using Pryanigram.Pipeline;

namespace Pryanigram.ArgumentsConversion.Converter.Default;

public sealed class ArgumentConverter(
    ReadOnlyDictionary<string, Func<string, IServiceProvider, Task<object>>> converters) : IArgumentConverter
{
    private readonly ReadOnlyDictionary<string, Func<string, IServiceProvider, Task<object>>> _converters = converters;
    
    public async Task<object?> Convert(FlowContext context)
    {
        if (!_converters.TryGetValue(context.Command!, out var converter))
        {
            return null;
        }

        try
        {
            return await converter(context.Arguments!, context.ServiceProvider);
        }
        catch (Exception e)
        {
            var conversionException = new ConversionException(context.Arguments!, e);

            throw conversionException;
        }
    }
}
