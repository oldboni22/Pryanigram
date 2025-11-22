using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Pryanigram.Exceptions.Command;

namespace Pryanigram.Handler.Provider.Default;

internal static class HandlerProviderBuilderUtilities
{
    internal static ReadOnlyDictionary<string, Func<IServiceProvider, ITelegramMessageHandler>> BuildDictionary(
        IEnumerable<Assembly> assemblies)
    {
        var handlerTypes = assemblies
            .SelectMany(assembly => assembly
                .GetTypes()
                .WhereTypesAreValid());
        
        var dictionary = new ConcurrentDictionary<string, Func<IServiceProvider, ITelegramMessageHandler>>();

        Parallel.ForEach(handlerTypes, handlerType =>
        {
            var commandAttribute = Attribute.GetCustomAttribute(handlerType, typeof(FromCommandAttribute)) as FromCommandAttribute;
            var command = commandAttribute!.Message;

            if (string.IsNullOrEmpty(command))
            {
                throw new EmptyCommandException();
            }

            if (!dictionary.TryAdd(command, BuildProviderInvoker(handlerType)))
            {
                throw new DuplicateCommandException(command);
            }
        });
        
        return new(dictionary);
    }
    
    private static IEnumerable<Type> WhereTypesAreValid(this IEnumerable<Type> types)
    {
        return types.Where(t => Attribute.IsDefined(t, typeof(FromCommandAttribute)))
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .Where(t => t.IsSubclassOf(typeof(ITelegramMessageHandler)));
    }
    
    private static Func<IServiceProvider, ITelegramMessageHandler> BuildProviderInvoker(Type handlerType)
    {
        var factory = ActivatorUtilities.CreateFactory(handlerType, Type.EmptyTypes);
        
        return serviceProvider => (ITelegramMessageHandler)factory(serviceProvider, null);
    }
}