using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Pryanigram.Exceptions.Command;

namespace Pryanigram.ArgumentsConversion.Converter.Default;

internal static class ConverterBuilderUtilities
{
    internal static ReadOnlyDictionary<string, Func<string, IServiceProvider, Task<object>>> BuildConverters(IEnumerable<Assembly> assemblies)
    {
        var providerTypes = assemblies
            .SelectMany(assembly => assembly
                .GetTypes()
                .WhereTypesAreValid());

        ConcurrentDictionary<string, Func<string, IServiceProvider, Task<object>>> converters = new();

        foreach (var providerType in providerTypes)
        {
            var commandAttributes = Attribute.GetCustomAttributes(providerType, typeof(FromCommandAttribute));
            var commands = commandAttributes.Select(attribute => ((FromCommandAttribute)attribute).Message);

            Parallel.ForEach(commands, command =>
            {
                if (string.IsNullOrEmpty(command))
                {
                    throw new EmptyCommandException();
                }
                
                if (!converters.TryAdd(command, BuildConverterInvoker(providerType)))
                {
                    throw new DuplicateCommandException(command);
                }        
            });
        }
        
        return new(converters);
    }

    private static Func<string, IServiceProvider, Task<object>> BuildConverterInvoker(Type converterType)
    {
        var factory = ActivatorUtilities.CreateFactory(converterType, Type.EmptyTypes);

        return async (arguments, serviceProvider) =>
        {
            var instance = factory(serviceProvider, null);

            if (instance is IArgumentsConversionProvider provider)
            {
                return await provider.ConvertUntypedAsync(arguments);
            }

            throw new InvalidOperationException();
        };
    }
    
    private static IEnumerable<Type> WhereTypesAreValid(this IEnumerable<Type> types)
    {
        return types
            .Where(t => Attribute.IsDefined(t, typeof(FromCommandAttribute)))
            .Where(t => t is { IsInterface: false, IsAbstract: false })
            .Where(t => t.IsSubclassOfConverterProvider());
    }
    
    private static bool IsSubclassOfConverterProvider(this Type type)
    {
        var converterProviderType = typeof(ArgumentsConversionProvider<>);

        Type? currentType = type;

        while (currentType != null && currentType != typeof(object))
        {
            if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == converterProviderType)
            {
                return true;
            }
            
            currentType = currentType.BaseType;
        } 
        
        return false;
    }
}