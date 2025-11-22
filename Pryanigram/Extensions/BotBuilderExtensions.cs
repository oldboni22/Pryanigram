using Pryanigram.Bot;
using Telegram.Bot;

namespace Pryanigram.Extensions;

public static class BotBuilderExtensions
{
    public static BotBuilder WithDefaultClient(this BotBuilder builder, string apiToken)
    {
        return builder.WithTelegramClient(_ => new TelegramBotClient(apiToken));
    }
}