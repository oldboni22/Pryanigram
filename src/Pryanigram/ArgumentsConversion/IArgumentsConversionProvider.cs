namespace Pryanigram.ArgumentsConversion;

public interface IArgumentsConversionProvider
{
    internal Task<object> ConvertUntypedAsync(string arguments);
}
