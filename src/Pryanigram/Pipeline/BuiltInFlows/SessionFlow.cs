using Pryanigram.Handler.Provider.Contract;
using Pryanigram.ScopedBag;
using Pryanigram.ScopedBag.Contract;
using Pryanigram.SessionManagement.SessionProvider.Contract;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Pryanigram.Pipeline.BuiltInFlows;

public sealed class SessionFlow(
    IHandlerProvider handlerProvider) : IFlowEntry
{
    public async Task InvokeAsync(FlowContext context, FlowDelegate next)
    {
        if (string.IsNullOrEmpty(context.Command)
            || context.ChatId is null
            || context.UserId is null)
        {
            await next(context);
            return;
        }

        var sessionProvider = context.ServiceProvider.GetRequiredService<ISessionProvider>();
        
        if (handlerProvider.HasHandler(context.Command))
        {
            await sessionProvider.DeleteSessionAsync(context.UserId.Value, context.ChatId.Value);
            
            await next(context);
            return;
        }
        
        var session = await sessionProvider.GetSessionAsync(context.UserId.Value, context.ChatId.Value);

        if (session is not null)
        {
            context.Command = session.SessionName;
            context.Arguments = context.FullText;

            var scopedBag = context.ServiceProvider.GetRequiredService<IScopedBag>();
            scopedBag.Set<UserSession>(DefaultScopedBagKeys.SessionKey, session);
        }
        
        await next(context);
    }
}
