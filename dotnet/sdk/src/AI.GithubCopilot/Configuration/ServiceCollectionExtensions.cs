using System.Net;
using AI.GithubCopilot.Domain.Services;
using AI.GithubCopilot.Infrastructure.Mappers.Dtos;
using AI.GithubCopilot.Infrastructure.Services;
using AI.GithubCopilot.Infrastructure.Services.ToMigrate;
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
    public static IServiceCollection AddAiGithubCopilot(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDomain()
            .AddInfrastructure()
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
            .AddSingleton<GithubCopilotChatClient>();
    }
    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AiGithubOptions>(configuration.GetSection(nameof(AiGithubOptions)));
        return services;
    }
    private static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        return services
            .AddInfrastructureMappers()
            .AddInfrastructureServices();
    }

    private static IServiceCollection AddInfrastructureMappers(this IServiceCollection services)
    {
        services.AddSingleton<ChatCompletionMapper>();
        return services;
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        return services.AddGithubCopilotChatCompletionServices();
    }

    /// <summary>
    /// Registers AI Github Copilot services and options.
    /// Requires AiGithubCopilotUserProvider to be registered in the service collection.
    /// Requires IMemoryCache to be registered in the service collection.
    /// </summary>
    private static IServiceCollection AddGithubCopilotChatCompletionServices(this IServiceCollection services)
    {
        services
            .AddToMigrate()
            .TryAddTransient<GithubCopilotChatCompletionAuthorizationHandler>();
        services
            .AddHttpClient<GithubCopilotChatCompletion>()
            .AddHttpMessageHandler<GithubCopilotChatCompletionAuthorizationHandler>();


        return services;
    }

    private static IServiceCollection AddToMigrate(this IServiceCollection services)
    {
        services.TryAddTransient<HttpListener>();
        services.TryAddTransient<TaskCompletionSource<bool>>();
        services.TryAddSingleton<HttpClientRunner>();
        services.TryAddSingleton<EncryptedEnvironment>();
        services.TryAddSingleton<GithubAccessTokenStore>();
        services.TryAddTransient<GithubAuthenticator>();
        services.AddHttpClient<GithubAccessTokenProvider>();
        services.TryAddTransient<GithubCopilotTokenAuthorizationHandler>();
        services.AddHttpClient<GithubCopilotTokenProvider>()
            .AddHttpMessageHandler<GithubCopilotTokenAuthorizationHandler>();
        return services;
    }
}
