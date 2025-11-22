using Pryanigram.Pipeline;

namespace Pryanigram.SessionManagement.SessionProvider.Contract;

public interface ISessionProvider
{
    Task<UserSession?> GetSessionAsync(long userId, long chatId);
    
    Task DeleteSessionAsync(long userId, long chatId);

    Task SetSessionAsync(long userId, long chatId ,UserSession userSession);
}
