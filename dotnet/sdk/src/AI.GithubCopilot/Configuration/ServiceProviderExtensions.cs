using AI.GithubCopilot.Infrastructure.Services.ToMigrate;
using Microsoft.Extensions.DependencyInjection;

namespace AI.GithubCopilot.Configuration;

public static class ServiceProviderExtensions
{
    public static async Task UseAiGithubCopilotAsync(this IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var githubAccessTokenProvider = serviceProvider.GetRequiredService<GithubAccessTokenProvider>();
        await githubAccessTokenProvider.GetGithubAccessToken(cancellationToken);
    }
}
