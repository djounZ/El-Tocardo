using AI.GithubCopilot.Configuration;
using ElTocardo.Application.Configuration;

namespace ElTocardo.Infrastructure.Configuration;

public static class ServiceProviderExtensions
{
    public static async Task UseAddElTocardoInfrastructureAsync(this IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await serviceProvider.UseAiGithubCopilotAsync(cancellationToken);
        await serviceProvider.UseElTocardoApplicationAsync(cancellationToken);
    }
}
