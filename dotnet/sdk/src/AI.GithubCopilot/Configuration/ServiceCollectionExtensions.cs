using AI.GithubCopilot.Domain.Services;
using AI.GithubCopilot.Infrastructure.Mappers.Dtos;
using AI.GithubCopilot.Infrastructure.Services;
using AI.GithubCopilot.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AI.GithubCopilot.Configuration;

public static class ServiceCollectionExtensions
{

    /// <summary>
    /// Registers AI Github Copilot services and options.
    /// Requires AiGithubCopilotUserProvider to be registered in the service collection.
    /// Requires IMemoryCache to be registered in the service collection.
    /// </summary>
    public static IServiceCollection AddAiGithubCopilot<TGithubCopilotAccessTokenResponseDtoProvider, TGithubAccessTokenResponseDtoProvider>(this IServiceCollection services, IConfiguration configuration) where TGithubCopilotAccessTokenResponseDtoProvider: class, IGithubCopilotAccessTokenResponseDtoProvider where TGithubAccessTokenResponseDtoProvider: class, IGithubAccessTokenResponseDtoProvider
    {
        return services
            .AddDomain()
            .AddInfrastructure<TGithubCopilotAccessTokenResponseDtoProvider, TGithubAccessTokenResponseDtoProvider>()
            .AddOptions(configuration);
    }

    private static IServiceCollection AddDomain(this IServiceCollection services)
    {

        return services
            .DomainServices();
    }

    private static IServiceCollection DomainServices(this IServiceCollection services)
    {

        return services
            .AddTransient<GithubCopilotChatClient>();
    }
    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AiGithubOptions>(configuration.GetSection(nameof(AiGithubOptions)));
        return services;
    }
    private static IServiceCollection AddInfrastructure<TGithubCopilotAccessTokenResponseDtoProvider, TGithubAccessTokenResponseDtoProvider>(this IServiceCollection services) where TGithubCopilotAccessTokenResponseDtoProvider: class, IGithubCopilotAccessTokenResponseDtoProvider where TGithubAccessTokenResponseDtoProvider: class, IGithubAccessTokenResponseDtoProvider
    {

        return services
            .AddInfrastructureMappers()
            .AddInfrastructureServices<TGithubCopilotAccessTokenResponseDtoProvider, TGithubAccessTokenResponseDtoProvider>();
    }

    private static IServiceCollection AddInfrastructureMappers(this IServiceCollection services)
    {
        services.AddSingleton<ChatCompletionMapper>();
        return services;
    }

    private static IServiceCollection AddInfrastructureServices<TGithubCopilotAccessTokenResponseDtoProvider, TGithubAccessTokenResponseDtoProvider>(this IServiceCollection services) where TGithubCopilotAccessTokenResponseDtoProvider: class, IGithubCopilotAccessTokenResponseDtoProvider where TGithubAccessTokenResponseDtoProvider: class, IGithubAccessTokenResponseDtoProvider
    {
        services.TryAddTransient<IGithubAccessTokenResponseDtoProvider, TGithubAccessTokenResponseDtoProvider>();
        services.TryAddTransient<IGithubCopilotAccessTokenResponseDtoProvider, TGithubCopilotAccessTokenResponseDtoProvider>();

        services.AddHttpClient<GithubAccessTokenResponseHttpClient>();
        services.TryAddTransient<GithubCopilotTokenAuthorizationHandler>();
        services.AddHttpClient<GithubCopilotAccessTokenResponseDtoHttpClient>()
            .AddHttpMessageHandler<GithubCopilotTokenAuthorizationHandler>();
        return services
            .AddGithubCopilotChatCompletionServices();
    }

    /// <summary>
    /// Registers AI Github Copilot services and options.
    /// Requires AiGithubCopilotUserProvider to be registered in the service collection.
    /// Requires IMemoryCache to be registered in the service collection.
    /// </summary>
    private static IServiceCollection AddGithubCopilotChatCompletionServices(this IServiceCollection services)
    {
        services
            .TryAddTransient<GithubCopilotChatCompletionAuthorizationHandler>();
        services
            .AddHttpClient<GithubCopilotChatCompletion>()
            .AddHttpMessageHandler<GithubCopilotChatCompletionAuthorizationHandler>();


        return services;
    }
}
