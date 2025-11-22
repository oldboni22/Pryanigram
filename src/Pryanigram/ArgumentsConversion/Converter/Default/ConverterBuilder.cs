using System.Reflection;

namespace Pryanigram.ArgumentsConversion.Converter.Default;

public static class ConverterBuilder
{
    internal static ArgumentConverter Build(params Assembly[] assemblies)
    {
        return new ArgumentConverter(ConverterBuilderUtilities.BuildConverters(assemblies));
    }
}
