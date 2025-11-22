using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pryanigram.ArgumentsConversion;
using Pryanigram.Pipeline;
using Telegram.Bot;

namespace Pryanigram.Bot;

public sealed class BotBuilder
{
    public BotBuilder()
    {
        FlowBuilder = new(this);
    }

    private bool _isBuilt = false;
    
    private CancellationTokenSource? _cancellationTokenSource = null;
    
    private Func<ITelegramBotClient, Exception, CancellationToken, Task> _errorHandler = 
        (ITelegramBotClient client, Exception ex, CancellationToken cancellationToken) => Task.CompletedTask;
    
    public FlowBuilder FlowBuilder { get; }
    
    private readonly IServiceCollection _services = new ServiceCollection();
    
    public IServiceCollection Services
    {
        get
        {
            SecureBuilt();
            return _services;
        }
    }

    public BotBuilder WithTelegramClient(Func<IServiceProvider, ITelegramBotClient> factory)
    {
        Services.TryAddSingleton(factory);

        return this;
    }
    
    public BotBuilder WithCancellationTokenSource(CancellationTokenSource cancellationTokenSource)
    {
        _cancellationTokenSource = cancellationTokenSource;

        return this;
    }
    
    public BotBuilder UseExceptionHandler(Func<ITelegramBotClient, Exception, CancellationToken, Task> handler)
    {
        _errorHandler = handler;
        
        return this;
    }
    
    public Bot Build()
    {
        SecureBuilt();
        
        _isBuilt = true;
        
        var sp = _services.BuildServiceProvider();

        var client = sp.GetRequiredService<ITelegramBotClient>();
        var flow = FlowBuilder.Build(sp);
        
        return new Bot(
            client,
            sp,
            flow,
            _errorHandler,
            _cancellationTokenSource
            );
    }

    internal void SecureBuilt()
    {
        if (_isBuilt)
        {
            throw new InvalidOperationException("Bot is already built.");
        }
    }
    
}