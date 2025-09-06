using ElTocardo.API.ToMigrate;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Repositories;
using ElTocardo.Infrastructure.Configuration;

namespace ElTocardo.API.Configuration;

public static class ServiceProviderExtensions
{
    internal static async Task UseElTocardoApiAsync(this IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await serviceProvider.UseAiGithubCopilotAsync(cancellationToken);
        await serviceProvider.UseElTocardoInfrastructureAsync(cancellationToken);
    }



    public static async Task UseAiGithubCopilotAsync(this IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        using var serviceScope = serviceScopeFactory.CreateScope();
        var githubAccessTokenProvider = serviceScope.ServiceProvider.GetRequiredService<GithubAccessTokenProvider>();
        await githubAccessTokenProvider.GetGithubAccessToken(cancellationToken);
    }
}
