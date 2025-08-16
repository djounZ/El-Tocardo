using AI.GithubCopilot.Infrastructure.Mappers.Dtos;
using AI.GithubCopilot.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AI.GithubCopilot.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAiGithubCopilot(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddInfrastructure()
            .AddOptions(configuration);
        return services;
    }
    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AiGithubOptions>(configuration.GetSection(nameof(AiGithubOptions)));
        return services;
    }
    private static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        return services
            .AddMappers();
    }

    private static IServiceCollection AddMappers(this IServiceCollection services)
    {
        services.AddSingleton<ChatCompletionMapper>();
        return services;
    }
}
