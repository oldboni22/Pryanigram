using Pryanigram.Pipeline;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Pryanigram.Handler;

public abstract class TelegramMessageHandler
{
    public abstract Task HandleAsync(
        FlowContext context,
        CancellationToken cancellationToken = default);
}
