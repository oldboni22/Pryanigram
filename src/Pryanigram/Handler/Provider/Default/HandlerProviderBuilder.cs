using System.Reflection;

namespace Pryanigram.Handler.Provider.Default;

public static class HandlerProviderBuilder
{
    internal static Handler.Provider.Default.HandlerProvider Build(params Assembly[] assemblies)
    {
        return new Handler.Provider.Default.HandlerProvider(HandlerProviderBuilderUtilities.BuildDictionary(assemblies));
    }
}
