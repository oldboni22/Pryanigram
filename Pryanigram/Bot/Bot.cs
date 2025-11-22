using Microsoft.Extensions.DependencyInjection;
using Pryanigram.Pipeline;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Pryanigram.Bot;

public sealed class Bot
{
    internal Bot(
        ITelegramBotClient botClient,
        IServiceProvider serviceProvider,
        FlowDelegate flow,
        Func<ITelegramBotClient, Exception, CancellationToken, Task> errorHandler,
        CancellationTokenSource? cancellationTokenSource = null)
    {
        _botClient = botClient;
        _cancellationTokenSource = cancellationTokenSource;
        _serviceProvider = serviceProvider;
        _flow = flow;

        ErrorHandler = errorHandler;
    }

    private readonly IServiceProvider _serviceProvider;

    private readonly FlowDelegate _flow;
    private Func<ITelegramBotClient, Exception, CancellationToken, Task> ErrorHandler { get; init; }

    private readonly CancellationTokenSource? _cancellationTokenSource;

    private readonly ITelegramBotClient _botClient;

    public void Start()
    {
        _botClient.StartReceiving(
            ProcessMessageAsync,
            ErrorHandler,
            cancellationToken: _cancellationTokenSource?.Token ?? CancellationToken.None);
    }

    private async Task ProcessMessageAsync(ITelegramBotClient client, Update update,
        CancellationToken cancellationToken = default)
    {
        var scope = _serviceProvider.CreateScope();
        try
        {
            var context = new FlowContext
            {
                ServiceProvider = scope.ServiceProvider,
                Update = update
            };

            await _flow(context);
        }
        finally
        {
            scope.Dispose();
        }
    }
}