using ElTocardo.API.ToMigrate;
using ElTocardo.Infrastructure.Configuration;

namespace ElTocardo.API.Configuration;

public static class ServiceProviderExtensions
{
    extension(IServiceProvider serviceProvider)
    {
        internal async Task UseElTocardoApiAsync(CancellationToken cancellationToken)
        {
            //await serviceProvider.UseAiGithubCopilotAsync(cancellationToken);
            await serviceProvider.UseElTocardoInfrastructureAsync(cancellationToken);
        }

        public async Task UseAiGithubCopilotAsync(CancellationToken cancellationToken)
        {
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using var serviceScope = serviceScopeFactory.CreateScope();
            var githubAccessTokenProvider = serviceScope.ServiceProvider.GetRequiredService<GithubAccessTokenProvider>();
            await githubAccessTokenProvider.GetGithubAccessToken("",cancellationToken);
        }
    }
}
