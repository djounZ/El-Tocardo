namespace AI.GithubCopilot.Infrastructure.Services;

public sealed class AiGithubCopilotUserProvider(Func<string> getCurrentUser)
{
    public Func<string> GetCurrentUser { get; } = getCurrentUser;
}
