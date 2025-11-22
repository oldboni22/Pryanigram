using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Pryanigram.Pipeline.BuiltInFlows;

public sealed class ParseMessageFlow : IFlowEntry
{
    public async Task InvokeAsync(FlowContext context, FlowDelegate next)
    {
        (long? userId, long? chatId) = GetUsedIdAndChatId(context.Update);
        
        context.UserId = userId;
        context.ChatId = chatId;
        
        string? textToParse = context.Update.Type switch
        {
            UpdateType.Message => context.Update.Message?.Text,
            UpdateType.CallbackQuery => context.Update.CallbackQuery?.Data,
            UpdateType.InlineQuery => context.Update.InlineQuery?.Query,
            _ => null
        };
        
        if (!string.IsNullOrEmpty(textToParse))
        {
            (string fullText, string command, string arguments) = GetCommandAndArguments(textToParse);
            
            context.FullText = fullText;
            context.Command = command;
            context.Arguments = arguments;
        }
        
        await next(context);
    }
    
    private static (long? userId, long? chatId) GetUsedIdAndChatId(Update update)
    {
        return update.Type switch
        {
            UpdateType.Message => (update.Message?.From?.Id, update.Message?.Chat.Id),
            UpdateType.CallbackQuery => (update.CallbackQuery?.From?.Id, update.CallbackQuery?.Message?.Chat.Id),
            UpdateType.InlineQuery => (update.InlineQuery?.From?.Id, null),
            _ => (null, null)
        };
    }
    
    private static (string fullText, string command, string arguments) GetCommandAndArguments(string text)
    {
        ReadOnlySpan<char> messageSpan = text.AsSpan();
        messageSpan = messageSpan.Trim();
        
        var spaceIndex = messageSpan.IndexOf(' ');

        if (spaceIndex == -1)
        {
            var final =  messageSpan.ToString();
            return (final, final, string.Empty);
        }

        var command = messageSpan.Slice(0, spaceIndex);
        
        var arguments = messageSpan.Length > spaceIndex + 1 
            ? messageSpan.Slice(spaceIndex + 1) 
            : default;

        return (messageSpan.ToString(), command.ToString(), arguments.ToString());
    }
}
