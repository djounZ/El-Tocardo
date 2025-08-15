using AI.GithubCopilot.Configuration;
using ElTocardo.Application.Configuration;
using ElTocardo.Infrastructure.Mappers.Dtos.AI;
using ElTocardo.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElTocardo.Infrastructure.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElTocardoInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ElTocardoInfrastructureOptions>(
            configuration.GetSection(nameof(ElTocardoInfrastructureOptions)));
        services.AddAiGithubCopilot(configuration);
        services.AddElTocardoApplication(configuration);
        return services;
    }

    private static IServiceCollection AddMappers(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDtos(configuration);
        return services;
    }

    private static IServiceCollection AddDtos(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAi(configuration);
        return services;
    }

    private static IServiceCollection AddAi(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<AiChatCompletionMapper>();
        services.AddSingleton<AiContentMapper>();
        return services;
    }
}
