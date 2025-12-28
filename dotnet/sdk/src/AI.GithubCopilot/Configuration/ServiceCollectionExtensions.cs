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

    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers AI Github Copilot services and options.
        /// Requires AiGithubCopilotUserProvider to be registered in the service collection.
        /// Requires IMemoryCache to be registered in the service collection.
        /// </summary>
        public IServiceCollection AddAiGithubCopilot<TGithubCopilotAccessTokenResponseDtoProvider, TGithubAccessTokenResponseDtoProvider>(IConfiguration configuration) where TGithubCopilotAccessTokenResponseDtoProvider: class, IGithubCopilotAccessTokenResponseDtoProvider where TGithubAccessTokenResponseDtoProvider: class, IGithubAccessTokenResponseDtoProvider
        {
            return services
                .AddDomain()
                .AddInfrastructure<TGithubCopilotAccessTokenResponseDtoProvider, TGithubAccessTokenResponseDtoProvider>()
                .AddOptions(configuration);
        }

        private IServiceCollection AddDomain()
        {

            return services
                .DomainServices();
        }

        private IServiceCollection DomainServices()
        {

            return services
                .AddTransient<GithubCopilotChatClient>();
        }

        private IServiceCollection AddOptions(IConfiguration configuration)
        {
            services.Configure<AiGithubOptions>(configuration.GetSection(nameof(AiGithubOptions)));
            return services;
        }

        private IServiceCollection AddInfrastructure<TGithubCopilotAccessTokenResponseDtoProvider, TGithubAccessTokenResponseDtoProvider>() where TGithubCopilotAccessTokenResponseDtoProvider: class, IGithubCopilotAccessTokenResponseDtoProvider where TGithubAccessTokenResponseDtoProvider: class, IGithubAccessTokenResponseDtoProvider
        {

            return services
                .AddInfrastructureMappers()
                .AddInfrastructureServices<TGithubCopilotAccessTokenResponseDtoProvider, TGithubAccessTokenResponseDtoProvider>();
        }

        private IServiceCollection AddInfrastructureMappers()
        {
            services.AddSingleton<ChatCompletionMapper>();
            return services;
        }

        private IServiceCollection AddInfrastructureServices<TGithubCopilotAccessTokenResponseDtoProvider, TGithubAccessTokenResponseDtoProvider>() where TGithubCopilotAccessTokenResponseDtoProvider: class, IGithubCopilotAccessTokenResponseDtoProvider where TGithubAccessTokenResponseDtoProvider: class, IGithubAccessTokenResponseDtoProvider
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
        private IServiceCollection AddGithubCopilotChatCompletionServices()
        {
            services
                .TryAddTransient<GithubCopilotChatCompletionAuthorizationHandler>();
            services
                .AddHttpClient<GithubCopilotChatCompletion>()
                .AddHttpMessageHandler<GithubCopilotChatCompletionAuthorizationHandler>();


            return services;
        }
    }
}
