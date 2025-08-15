using AI.GithubCopilot.Configuration;
using ElTocardo.Application.Configuration;
using ElTocardo.Application.Services;
using ElTocardo.Infrastructure.Mappers.Dtos.AI;
using ElTocardo.Infrastructure.Options;
using ElTocardo.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElTocardo.Infrastructure.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElTocardoInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddElTocardoApplication(configuration)
            .AddAiGithubCopilot(configuration)
            .AddOptions(configuration)
            .AddMappers(configuration)
            .AddServices(configuration);
        return services;
    }


    private static IServiceCollection AddOptions(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ElTocardoInfrastructureOptions>(
            configuration.GetSection(nameof(ElTocardoInfrastructureOptions)));
        return services;
    }
    private static IServiceCollection AddServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IChatCompletionsService, ChatCompletionsService>();
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
