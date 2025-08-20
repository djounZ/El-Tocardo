using AI.GithubCopilot.Configuration;
using ElTocardo.Application.Commands.PresetChatOptions;
using ElTocardo.Application.Configuration;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Handlers.PresetChatOptions;
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Commands;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Handlers;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Mappers;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Queries;
using ElTocardo.Application.Queries.PresetChatOptions;
using ElTocardo.Application.Services;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using ElTocardo.Domain.Repositories;
using ElTocardo.Infrastructure.Data;
using ElTocardo.Infrastructure.Mappers.Dtos.AI;
using ElTocardo.Infrastructure.Mappers.Dtos.ModelContextProtocol;
using ElTocardo.Infrastructure.Options;
using ElTocardo.Infrastructure.Repositories;
using ElTocardo.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OllamaSharp;

namespace ElTocardo.Infrastructure.Configuration;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers  ElTocardo Infrastructure services and options.
    ///     Requires AiGithubCopilotUserProvider to be registered in the service collection.
    ///     Requires IMemoryCache to be registered in the service collection.
    /// </summary>
    public static IServiceCollection AddElTocardoInfrastructure(this IServiceCollection services,
        IConfiguration configuration, Action<DbContextOptionsBuilder> configureDbContext)
    {
        return services
            .AddElTocardoApplication(configuration)
            .AddAiGithubCopilot(configuration)
            .AddOllamaApiClient(configuration)
            .AddDatabase(configureDbContext)
            .AddRepositories()
            .AddValidation()
            .AddCommandQueryHandlers()
            .AddOptions(configuration)
            .AddMappers()
            .AddServices();
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
        services.AddTransient<IAiProviderService, AiProviderService>();
        services.AddTransient<ChatClientStore>();
        services.AddTransient<ChatClientProvider>();
        services.AddTransient<AiToolsProviderService>();
        services.AddTransient<IChatCompletionsService, ChatCompletionsService>();

        // New CQRS-based services
        services.AddScoped<IMcpServerConfigurationService, McpServerConfigurationService>();
        services.AddScoped<IPresetChatOptionsService, PresetChatOptionsService>();

        services.AddTransient<IMcpClientToolsService, McpClientToolsService>();
        services.AddTransient<ClientTransportFactoryService>();
        return services;
    }

    private static IServiceCollection AddMappers(this IServiceCollection services)
    {
        return services.AddDtos();
    }

    private static IServiceCollection AddDtos(this IServiceCollection services)
    {
        services.AddAi()
            .TryAddSingleton<ModelContextProtocolMapper>();
        return services;
    }

    private static IServiceCollection AddAi(this IServiceCollection services)
    {
        services.TryAddSingleton<AiChatCompletionMapper>();
        services.TryAddSingleton<AiContentMapper>();
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureDbContext)
    {
        services.AddDbContext<ApplicationDbContext>(configureDbContext);
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IMcpServerConfigurationRepository, McpServerConfigurationRepository>();
        services.AddScoped<IPresetChatOptionsRepository, PresetChatOptionsRepository>();
        return services;
    }

    private static IServiceCollection AddCommandQueryHandlers(this IServiceCollection services)
    {
        services.AddSingleton<McpServerConfigurationDomainDtoMapper>();
        services.AddSingleton<McpServerConfigurationDomainCommandMapper>();

        // MCP Command handlers
        services.AddScoped<ICommandHandler<CreateMcpServerCommand, Guid>, CreateMcpServerCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateMcpServerCommand>, UpdateMcpServerCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteMcpServerCommand>, DeleteMcpServerCommandHandler>();

        // MCP Query handlers
        services
            .AddScoped<IQueryHandler<GetAllMcpServersQuery, IDictionary<string, McpServerConfigurationItemDto>>,
                GetAllMcpServersQueryHandler>();
        services
            .AddScoped<IQueryHandler<GetMcpServerByNameQuery, McpServerConfigurationItemDto>,
                GetMcpServerByNameQueryHandler>();

        // PresetChatOptions Command handlers
        services
            .AddScoped<ICommandHandler<CreatePresetChatOptionsCommand, Guid>, CreatePresetChatOptionsCommandHandler>();
        services
            .AddScoped<ICommandHandler<UpdatePresetChatOptionsCommand, bool>, UpdatePresetChatOptionsCommandHandler>();
        services
            .AddScoped<ICommandHandler<DeletePresetChatOptionsCommand, bool>, DeletePresetChatOptionsCommandHandler>();

        // PresetChatOptions Query handlers
        services
            .AddScoped<IQueryHandler<GetAllPresetChatOptionsQuery, List<PresetChatOptionsDto>>,
                GetAllPresetChatOptionsQueryHandler>();
        services
            .AddScoped<IQueryHandler<GetPresetChatOptionsByNameQuery, PresetChatOptionsDto>,
                GetPresetChatOptionsByNameQueryHandler>();

        return services;
    }

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateMcpServerCommand>();
        return services;
    }
}
