using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Pryanigram.Handler.Provider.Default;

internal static class HandlerProviderBuilderUtilities
{
    internal static ReadOnlyDictionary<string, Func<IServiceProvider, TelegramMessageHandler>> BuildDictionary(
        IEnumerable<Assembly> assemblies)
    {
        var handlerTypes = assemblies
            .SelectMany(assembly => assembly
                .GetTypes()
                .WhereTypesAreValid());
        
        var dictionary = new ConcurrentDictionary<string, Func<IServiceProvider, TelegramMessageHandler>>();

        Parallel.ForEach(handlerTypes, handlerType =>
        {
            var commandAttribute = Attribute.GetCustomAttribute(handlerType, typeof(FromCommandAttribute)) as FromCommandAttribute;
            var command = commandAttribute!.Message;

            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException(); //TODO custom ex
            }

            if (!dictionary.TryAdd(command, BuildProviderInvoker(handlerType)))
            {
                throw new ArgumentException(); //TODO custom ex
            }
        });
        
        return new(dictionary);
    }
    
    private static IEnumerable<Type> WhereTypesAreValid(this IEnumerable<Type> types)
    {
        return types.Where(t => Attribute.IsDefined(t, typeof(FromCommandAttribute)))
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .Where(t => t.IsSubclassOf(typeof(TelegramMessageHandler)));
    }
    
    private static Func<IServiceProvider, TelegramMessageHandler> BuildProviderInvoker(Type handlerType)
    {
        var factory = ActivatorUtilities.CreateFactory(handlerType, Type.EmptyTypes);
        
        return serviceProvider => (TelegramMessageHandler)factory(serviceProvider, null);
    }
}