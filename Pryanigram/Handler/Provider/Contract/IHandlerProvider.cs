using Pryanigram.Pipeline;

namespace Pryanigram.Handler.Provider.Contract;

public interface IHandlerProvider
{
    ITelegramMessageHandler? ConstructHandler(FlowContext context);

    bool HasHandler(string command);
}