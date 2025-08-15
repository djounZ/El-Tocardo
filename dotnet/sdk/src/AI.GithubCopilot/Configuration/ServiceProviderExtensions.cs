namespace AI.GithubCopilot.Configuration;

public static class ServiceProviderExtensions
{
    public static async Task UseAiGithubCopilotAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

}
