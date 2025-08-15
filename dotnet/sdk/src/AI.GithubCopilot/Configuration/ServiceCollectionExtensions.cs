using AI.GithubCopilot.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AI.GithubCopilot.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAiGithubCopilot(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AiGithubOptions>(configuration.GetSection(nameof(AiGithubOptions)));
        return services;
    }
}
