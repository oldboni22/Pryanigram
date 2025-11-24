using Pryanigram.Extensions;
using Pryanigram.MessageHandling;
using Pryanigram.Pipeline;
using Pryanigram.ScopedBag.Contract;

namespace Pryanigram.SessionManagement.SessionProvider.Contract;

public abstract class SessionHandle<TStep>(IScopedBag bag, ISessionProvider sessionProvider) 
    : MessageHandle where TStep : struct, Enum
{
    protected IScopedBag Bag { get; } = bag;
    
    public override async Task HandleAsync()
    {
        var session = Bag.Get<UserSession>(DefaultScopedBagKeys.SessionKey)
            ?? new UserSession(FlowContext.Command!, default(TStep)!.ToString(), new());
        
        if (!Enum.TryParse<TStep>(session.Step, out var currentStep))
        {
            throw new InvalidCastException($"The step {session.Step} is not supported.");
        }
        
        await OnStepAsync(session, currentStep);
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
        UserSession session,
        TStep step);
}
