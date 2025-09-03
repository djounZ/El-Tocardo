namespace ElTocardo.API.ToMigrate;

public sealed class AiGithubCopilotUserProvider(Func<string> getCurrentUser)
{
    public Func<string> GetCurrentUser { get; } = getCurrentUser;
}
