using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pryanigram.ArgumentsConversion;
using Pryanigram.ArgumentsConversion.Converter.Contract;
using Pryanigram.ArgumentsConversion.Converter.Default;
using Pryanigram.Handler.Provider.Contract;
using Pryanigram.Handler.Provider.Default;
using Pryanigram.Pipeline.BuiltInFlows;

namespace Pryanigram.Pipeline;

public sealed class FlowBuilder
{
    internal FlowBuilder(Bot.BotBuilder builder)
    {
        _botBuilder = builder;
    }

    private readonly Bot.BotBuilder _botBuilder;
    
    private readonly List<Func<IServiceProvider,FlowEntry>> _flowFactories = [];

    private readonly HashSet<Type> _registeredTypes = new();
    
    public FlowBuilder Use<TFlow>() where TFlow : FlowEntry
    {
        _botBuilder.SecureBuilt();
        EnforceUniqueTypes(typeof(TFlow));
        
        _flowFactories.Add(sp => ActivatorUtilities.CreateInstance<TFlow>(sp));
        return this;
    }
    
    public FlowBuilder Use(FlowEntry instance)
    {
        _botBuilder.SecureBuilt();
        EnforceUniqueTypes(instance.GetType());
        
        _flowFactories.Add(_ => instance);
        return this;
    }
    
    public FlowBuilder UseHandlers(params Assembly[] assemblies)
    {
        _botBuilder.Services.TryAddSingleton<IHandlerProvider>(_ => HandlerProviderBuilder.Build(assemblies));
        
        return Use<HandleMessageFlow>();
    }
    
    public FlowBuilder UseHandlers(Func<IHandlerProvider> customHandlerProviderFactory, params Assembly[] assemblies)
    {
        _botBuilder.Services.TryAddSingleton<IHandlerProvider>(customHandlerProviderFactory());
        
        return Use<HandleMessageFlow>();
    }
    
    public FlowBuilder UseConverters(params Assembly[] assemblies)
    {
        _botBuilder.Services.TryAddSingleton<IArgumentConverter>(_ => ConverterBuilder.Build(assemblies));
        
        return Use<ArgumentConversionFlow>();
    }
    
    public FlowBuilder UseConverters(Func<IArgumentConverter> customConverterFactory, params Assembly[] assemblies)
    {
        _botBuilder.Services.TryAddSingleton<IArgumentConverter>(customConverterFactory());
        
        return Use<ArgumentConversionFlow>();
    }

    private void EnforceUniqueTypes(Type type)
    {
        if (!_registeredTypes.Add(type))
        {
            throw new ArgumentException($"The type {type} has already been registered."); //TODO custom ex
        }
    }
    
    internal FlowDelegate Build(IServiceProvider serviceProvider)
    {
        FlowDelegate pipeline = (context) => Task.CompletedTask;
        
        var instances = _flowFactories.Select(factory => factory(serviceProvider)).ToList();
        
        foreach (var flow in Enumerable.Reverse(instances))
        {
            var currentNext = pipeline;
            pipeline = (context) => flow.InvokeAsync(context, currentNext);
        }

        return pipeline;
    }
}