using ElTocardo.Application.Services;
using AI.GithubCopilot.Configuration;
using AI.GithubCopilot.Infrastructure.Services;
using ElTocardo.Application.Configuration;
using ElTocardo.Domain.Mediator.ConversationMediator.Repositories;
using ElTocardo.Infrastructure.EntityFramework.Configuration;
using ElTocardo.Infrastructure.EntityFramework.Mediator;
using ElTocardo.Infrastructure.Mediator.MongoDb.Repositories.Conversation;
using ElTocardo.Infrastructure.Options;
using ElTocardo.Infrastructure.Services;
using ElTocardo.Infrastructure.Services.Endpoints;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using OllamaSharp;

namespace ElTocardo.Infrastructure.Configuration;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers  ElTocardo Infrastructure services and options.
        ///     Requires AiGithubCopilotUserProvider to be registered in the service collection.
        ///     Requires IMemoryCache to be registered in the service collection.
        /// </summary>
        public IServiceCollection AddElTocardoInfrastructure<TApplicationDbContextOptionsConfiguration,TGithubCopilotAccessTokenResponseDtoProvider, TGithubAccessTokenResponseDtoProvider>(IConfiguration configuration, MongoClientSettings mongoClientSettings, string mongoDatabaseName, string applicationName) where TApplicationDbContextOptionsConfiguration : class, IDbContextOptionsConfiguration<ApplicationDbContext>  where TGithubCopilotAccessTokenResponseDtoProvider: class, IGithubCopilotAccessTokenResponseDtoProvider where TGithubAccessTokenResponseDtoProvider: class, IGithubAccessTokenResponseDtoProvider
        {
            return services
                .AddElTocardoInfrastructureEntityFramework<TApplicationDbContextOptionsConfiguration, ApplicationDbContext>(applicationName)
                .AddAiClients<TGithubCopilotAccessTokenResponseDtoProvider, TGithubAccessTokenResponseDtoProvider>(configuration)
                .AddMediator(mongoClientSettings, mongoDatabaseName)
                .AddOptions(configuration)
                .AddServices()
                .AddElTocardoApplication(configuration);
        }

        private IServiceCollection AddMediator(MongoClientSettings mongoClientSettings, string mongoDatabaseName)
        {

            return services
                .AddMongoDb(mongoClientSettings, mongoDatabaseName);
        }

        private IServiceCollection AddMongoDb(MongoClientSettings mongoClientSettings, string mongoDatabaseName)
        {
            // Register the serializer globally (usually in your startup code)
            BsonSerializer.RegisterSerializer(new AIContentSerializer());

            services.AddSingleton<IMongoClient>(_=>new MongoClient(mongoClientSettings));
            services.AddScoped<IMongoDatabase>(sp =>sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDatabaseName));
            return services;
        }

        private IServiceCollection AddAiClients<TGithubCopilotAccessTokenResponseDtoProvider, TGithubAccessTokenResponseDtoProvider>(IConfiguration configuration) where TGithubCopilotAccessTokenResponseDtoProvider: class, IGithubCopilotAccessTokenResponseDtoProvider where TGithubAccessTokenResponseDtoProvider: class, IGithubAccessTokenResponseDtoProvider
        {
            return services
                .AddAiGithubCopilot<TGithubCopilotAccessTokenResponseDtoProvider, TGithubAccessTokenResponseDtoProvider>(configuration)
                .AddOllamaApiClient(configuration);
        }

        private IServiceCollection AddOllamaApiClient(IConfiguration configuration)
        {
            services.Configure<OllamaOptions>(configuration.GetSection(nameof(OllamaOptions)));
            services.AddTransient<OllamaApiClient>(sc =>
            {
                var ollamaOptions = sc.GetRequiredService<IOptions<OllamaOptions>>().Value;

                return new OllamaApiClient(ollamaOptions.Uri, ollamaOptions.DefaultModel);
            });
            return services;
        }

        private IServiceCollection AddOptions(IConfiguration configuration)
        {
            services.Configure<ElTocardoInfrastructureOptions>(
                configuration.GetSection(nameof(ElTocardoInfrastructureOptions)));
            return services;
        }

        private IServiceCollection AddServices()
        {
            services.AddTransient<IAiProviderEndpointService, AiProviderEndpointService>();
            services.AddTransient<ChatClientStore>();
            services.AddTransient<ChatClientProvider>();
            services.AddTransient<AiToolsProviderService>();
            services.AddTransient<IChatCompletionsEndpointService, ChatCompletionsEndpointService>();
            services.AddTransient<IMcpClientToolsEndpointService, McpClientToolsEndpointService>();
            return
                services
                    .AddTransient<ClientTransportFactoryService>()
                    .AddConversationService()
                ;
        }

        private IServiceCollection AddConversationService()
        {
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IConversationEndpointService, ConversationEndpointService>();

            return services;
        }
    }
}
