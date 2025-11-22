using Pryanigram.Handler;
using Pryanigram.Pipeline;
using Pryanigram.ScopedBag;
using Pryanigram.ScopedBag.Contract;

namespace Pryanigram.SessionManagement.SessionProvider.Contract;

public abstract class SessionHandler<TStep>(IScopedBag bag, ISessionProvider sessionProvider) 
    : ITelegramMessageHandler where TStep : struct, Enum
{
    protected IScopedBag Bag { get; } = bag;
    
    public async Task HandleAsync(FlowContext context, CancellationToken cancellationToken = default)
    {
        var session = Bag.Get<UserSession>(DefaultScopedBagKeys.SessionKey)
            ?? new UserSession(context.Command!, default(TStep)!.ToString(), new());
        
        if (!Enum.TryParse<TStep>(session.Step, out var currentStep))
        {
            throw new InvalidCastException($"The step {session.Step} is not supported.");
        }
        
        await OnStepAsync(context, session, currentStep, cancellationToken);
    }
    
    protected async Task TransitionToStepAsync(FlowContext context, UserSession currentSession, TStep step)
    {
        var newSession = currentSession with
        {
            Step = step.ToString()
        };
        
        await sessionProvider.SetSessionAsync(context.UserId!.Value, context.ChatId!.Value, newSession);
    }

    protected async Task EndSessionAsync(FlowContext context)
    {
        await sessionProvider.DeleteSessionAsync(context.UserId!.Value, context.ChatId!.Value);
    }
    
    protected abstract Task OnStepAsync(
        FlowContext context, 
        UserSession session,
        TStep step,
        CancellationToken cancellationToken = default);
}
