using Pryanigram.Pipeline;

namespace Pryanigram.MessageHandling.Provider.Contract;

public interface IMessageHandler
{
    Task HandleMessage(FlowContext context);

    bool HasCommand(string command);
}
