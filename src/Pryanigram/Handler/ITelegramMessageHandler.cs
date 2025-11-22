using Pryanigram.Pipeline;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Pryanigram.Handler;

public interface ITelegramMessageHandler
{
    public abstract Task HandleAsync(
        FlowContext context,
        CancellationToken cancellationToken = default);
}
