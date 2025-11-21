using Pryanigram.Pipeline;

namespace Pryanigram.Handler.Provider.Contract;

public interface IHandlerProvider
{
    TelegramMessageHandler? ConstructHandler(FlowContext context);

    bool HasHandler(string command);
}