using AI.GithubCopilot.Configuration;
using ElTocardo.Application.Configuration;

namespace ElTocardo.Infrastructure.Configuration;

public static class ServiceProviderExtensions
{
    public static async Task<IServiceProvider> UseElTocardoInfrastructureAsync(this IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await serviceProvider.UseAiGithubCopilotAsync(cancellationToken);
        await serviceProvider.UseElTocardoApplicationAsync(cancellationToken);
        return serviceProvider;
    }
}
