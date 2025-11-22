using Pryanigram.Pipeline;

namespace Pryanigram.ArgumentsConversion.Converter.Contract;

public interface IArgumentConverter
{
    Task<object?> Convert(FlowContext context);
}