namespace Pryanigram.ArgumentsConversion;

public abstract class ArgumentsConversionProvider<T> : IArgumentsConversionProvider where T : class
{
    public abstract Task<T> ConvertAsync(string arguments);
    
    async Task<object> IArgumentsConversionProvider.ConvertUntypedAsync(string arguments)
    {
        var result = await ConvertAsync(arguments);

        if (result is null)
        {
            throw new InvalidOperationException(); //TODO custom ex
        }
        
        return result;
    }
}
