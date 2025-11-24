using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Pryanigram.Exceptions.Command;
using Pryanigram.Pipeline;

namespace Pryanigram.MessageHandling.Provider.Default;

internal static class HandlerBuilderUtilities
{
    internal static ReadOnlyDictionary<string, Func<IServiceProvider, FlowContext, Task>> BuildDictionary(
        IEnumerable<Assembly> assemblies)
    {
        var handlerTypes = assemblies
            .SelectMany(assembly => assembly
                .GetTypes()
                .WhereTypesAreValid());
        
        var dictionary = new ConcurrentDictionary<string, Func<IServiceProvider, FlowContext, Task>>();

        Parallel.ForEach(handlerTypes, handlerType =>
        {
            if (handlerType.IsAssignableTo(typeof(MessageHandle)))
            {
                HandleHandle(handlerType, dictionary);
            }
            else if (handlerType.IsAssignableTo(typeof(ControllerBase)))
            {
                HandleController(handlerType, dictionary);
            }
        });
        
        return new(dictionary);
    }

    private static IEnumerable<Type> WhereTypesAreValid(this IEnumerable<Type> types)
    {
        return types
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .Where(t =>
                (t.IsAssignableTo(typeof(MessageHandle)) && Attribute.IsDefined(t, typeof(FromCommandAttribute)))
                ||
                (t.IsAssignableTo(typeof(ControllerBase)) 
                 && t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                     .Any(m => Attribute.IsDefined(m, typeof(FromCommandAttribute))))
            );
    }
    
    private static void HandleController(
        Type type, ConcurrentDictionary<string, Func<IServiceProvider, FlowContext, Task>> dictionary)
    {
        var validMethods = type
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => Attribute.IsDefined(m, typeof(FromCommandAttribute)));

        foreach (var method in validMethods)
        {
            var commandAttribute = Attribute.GetCustomAttribute(method, typeof(FromCommandAttribute)) as FromCommandAttribute;
            var command = commandAttribute!.Message;

            if (string.IsNullOrEmpty(command))
            {
                throw new EmptyCommandException();
            }

            if (!dictionary.TryAdd(command, BuildControllerInvoker(type,  method)))
            {
                throw new DuplicateCommandException(command);
            }
        }
    }
    
    private static void HandleHandle(
        Type type, ConcurrentDictionary<string, Func<IServiceProvider, FlowContext, Task>> dictionary)
    {
        var commandAttribute = Attribute.GetCustomAttribute(type, typeof(FromCommandAttribute)) as FromCommandAttribute;
        var command = commandAttribute!.Message;

        if (string.IsNullOrEmpty(command))
        {
            throw new EmptyCommandException();
        }
        
        if (!dictionary.TryAdd(command, BuildHandleInvoker(type)))
        {
            throw new DuplicateCommandException(command);
        }
    }
    
    private static Func<IServiceProvider, FlowContext, Task> BuildHandleInvoker(Type handlerType)
    {
        var flowContextProperty = typeof(MessageHandle)
            .GetProperty(MessageHandle.FlowContextFieldName, 
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        
        var factory = ActivatorUtilities.CreateFactory(handlerType, Type.EmptyTypes);

        return async (sp, context) =>
        {
            var instance = (MessageHandle)factory(sp, null);

            try
            {
                flowContextProperty!.SetValue(instance, context);

                await instance.HandleAsync();
            }
            finally
            {
                await DisposeInstanceAsync(instance);
            }
        };
    }

    private static Func<IServiceProvider, FlowContext, Task> BuildControllerInvoker(Type controllerType, MethodInfo method)
    {
        if (!typeof(Task).IsAssignableFrom(method.ReturnType))
        {
            throw new InvalidOperationException(); //TODO
        }
        
        var flowContextProperty = typeof(ControllerBase)
            .GetProperty(ControllerBase.FlowContextFieldName, 
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        
        var factory = ActivatorUtilities.CreateFactory(controllerType, Type.EmptyTypes);
        
        var parameters = method.GetParameters();
        
        if (parameters.Length == 0)
        {
            
            return async (sp, context) =>
            {
                var instance = (ControllerBase)factory(sp, null);
                
                try
                {
                    flowContextProperty!.SetValue(instance, context);

                    await (Task)method.Invoke(instance, null)!;
                }
                finally
                {
                    await DisposeInstanceAsync(instance);
                }
            };
        }

        var expectedType = parameters[0].ParameterType;
        
        if (parameters.Length == 1)
        {
            return async (sp, context) =>
            {
                var instance = (ControllerBase)factory(sp, null);

                try
                {
                    flowContextProperty!.SetValue(instance, context);

                    if (!expectedType.IsInstanceOfType(context.ConvertedArguments))
                    {
                        throw new InvalidCastException();
                    }

                    await (Task)method.Invoke(instance, [context.ConvertedArguments])!;
                }
                finally
                {
                    await DisposeInstanceAsync(instance);
                }
            };
        }
        
        throw new InvalidOperationException(); //TODO
    }
    
    private static async ValueTask DisposeInstanceAsync(object instance)
    {
        if (instance is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else if (instance is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}