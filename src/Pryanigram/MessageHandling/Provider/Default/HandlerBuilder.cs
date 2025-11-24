using System.Reflection;

namespace Pryanigram.MessageHandling.Provider.Default;

public static class HandlerBuilder
{
    public static MessageHandler Build(params Assembly[] assemblies)
    {
        return new MessageHandler(HandlerBuilderUtilities.BuildDictionary(assemblies));
    }
}
