namespace Pryanigram.SessionManagement.SessionProvider.Contract;

public sealed record UserSession(string SessionName, string Step, Dictionary<string, string> Cached);
