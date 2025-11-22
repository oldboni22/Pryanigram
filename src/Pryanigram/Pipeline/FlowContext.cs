using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Pryanigram.Pipeline;

public sealed class FlowContext
{
    public required Update Update { get; init; }
    
    public required IServiceProvider ServiceProvider { get; init; }
    
    
    public long? UserId { get; set; }
    
    public long? ChatId { get; set; }
    
    public string? FullText { get; set; }
    
    public string? Command { get; set; }
    
    public string? Arguments { get; set; }
    

    public bool IsHandled { get; set; } = false;
    
    public object? ConvertedArguments { get; set; }
}
