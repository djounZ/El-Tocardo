using AI.GithubCopilot.Configuration;
using ElTocardo.Application.Configuration;
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Services;
using ElTocardo.Domain.Mediator.ConversationMediator.Repositories;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Repositories;
using ElTocardo.Infrastructure.Mediator.EntityFramework.ApplicationUserMediator;
using ElTocardo.Infrastructure.Mediator.EntityFramework.ApplicationUserMediator.Commands;
using ElTocardo.Infrastructure.Mediator.EntityFramework.ApplicationUserMediator.Handlers.Commands;
using ElTocardo.Infrastructure.Mediator.EntityFramework.ApplicationUserMediator.Handlers.Queries;
using ElTocardo.Infrastructure.Mediator.EntityFramework.ApplicationUserMediator.Queries;
using ElTocardo.Infrastructure.Mediator.EntityFramework.Data;
using ElTocardo.Infrastructure.Mediator.EntityFramework.Repositories;
using ElTocardo.Infrastructure.Mediator.MongoDb.Repositories;
using ElTocardo.Infrastructure.Mediator.MongoDb.Repositories.Conversation;
using ElTocardo.Infrastructure.Options;
using ElTocardo.Infrastructure.Services;
using ElTocardo.Infrastructure.Services.Endpoints;
using Microsoft.AspNetCore.Identity;
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
    /// <summary>
    ///     Registers  ElTocardo Infrastructure services and options.
    ///     Requires AiGithubCopilotUserProvider to be registered in the service collection.
    ///     Requires IMemoryCache to be registered in the service collection.
    /// </summary>
    public static IServiceCollection AddElTocardoInfrastructure<TApplicationDbContextOptionsConfiguration>(this IServiceCollection services,
        IConfiguration configuration, MongoClientSettings mongoClientSettings, string mongoDatabaseName) where TApplicationDbContextOptionsConfiguration : class, IDbContextOptionsConfiguration<ApplicationDbContext>
    {
        return services
            .AddAiClients(configuration)
            .AddMediator<TApplicationDbContextOptionsConfiguration>(mongoClientSettings, mongoDatabaseName)
            .AddOptions(configuration)
            .AddServices()
            .AddElTocardoApplication(configuration);
    }

    private static IServiceCollection AddMediator<TApplicationDbContextOptionsConfiguration>(this IServiceCollection services, MongoClientSettings mongoClientSettings, string mongoDatabaseName) where TApplicationDbContextOptionsConfiguration : class, IDbContextOptionsConfiguration<ApplicationDbContext>
    {

        return services
            .AddEntityFramework<TApplicationDbContextOptionsConfiguration>()
            .AddMongoDb(mongoClientSettings, mongoDatabaseName);
    }


    private static IServiceCollection AddEntityFramework<TApplicationDbContextOptionsConfiguration>(this IServiceCollection services) where TApplicationDbContextOptionsConfiguration : class, IDbContextOptionsConfiguration<ApplicationDbContext>
    {

        services
            .AddSingleton<IDbContextOptionsConfiguration<ApplicationDbContext>,
                TApplicationDbContextOptionsConfiguration>()
            .AddDbContext<ApplicationDbContext>();
        return services;
    }



    private static IServiceCollection AddMongoDb(this IServiceCollection services, MongoClientSettings mongoClientSettings, string mongoDatabaseName)
    {
        // Register the serializer globally (usually in your startup code)
        BsonSerializer.RegisterSerializer(new AIContentSerializer());

        services.AddSingleton<IMongoClient>(_=>new MongoClient(mongoClientSettings));
        services.AddScoped<IMongoDatabase>(sp =>sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDatabaseName));
        return services;
    }

    private static IServiceCollection AddAiClients(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddAiGithubCopilot(configuration)
            .AddOllamaApiClient(configuration);
    }

    private static IServiceCollection AddOllamaApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OllamaOptions>(configuration.GetSection(nameof(OllamaOptions)));
        services.AddTransient<OllamaApiClient>(sc =>
        {
            var ollamaOptions = sc.GetRequiredService<IOptions<OllamaOptions>>().Value;

            return new OllamaApiClient(ollamaOptions.Uri, ollamaOptions.DefaultModel);
        });
        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ElTocardoInfrastructureOptions>(
            configuration.GetSection(nameof(ElTocardoInfrastructureOptions)));
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
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
                .AddUserService()
                .AddMcpServerConfigurationService()
                .AddPresetChatOptionsService()
                .AddConversationService()
            ;
    }

    private static IServiceCollection AddPresetChatOptionsService(this IServiceCollection services)
    {
        services.AddScoped<IPresetChatOptionsRepository, PresetChatOptionsRepository>();
        return services;
    }

    private static IServiceCollection AddUserService(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // User Command handlers
        services.AddScoped<ICommandHandler<AuthenticateUserCommand>, AuthenticateUserCommandHandler>();
        services
            .AddScoped<ICommandHandler<InitiatePasswordResetCommand, string>, InitiatePasswordResetCommandHandler>();
        services.AddScoped<ICommandHandler<ConfirmPasswordResetCommand>, ConfirmPasswordResetCommandHandler>();
        services.AddScoped<ICommandHandler<UnregisterUserCommand>, UnregisterUserCommandHandler>();
        services.AddScoped<ICommandHandler<CreateUserCommand>, CreateUserCommandHandler>();

        // User Query handlers
        services.AddScoped<IQueryHandler<GetAllUsersQuery, ApplicationUser[]>, GetAllUsersQueryHandler>();

        services.AddScoped<IUserEndpointService, UserEndpointService>();
        return services;
    }




    private static IServiceCollection AddConversationService(this IServiceCollection services)
    {
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IConversationEndpointService, ConversationEndpointService>();

        return services;
    }

    private static IServiceCollection AddMcpServerConfigurationService(this IServiceCollection services)
    {
        services.AddScoped<IMcpServerConfigurationRepository, McpServerConfigurationRepository>();
        return services;
    }
}
