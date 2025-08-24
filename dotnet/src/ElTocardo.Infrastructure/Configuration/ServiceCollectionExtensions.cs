using AI.GithubCopilot.Configuration;
using ElTocardo.Application.Configuration;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Commands;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Handlers.Commands;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Handlers.Queries;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Mappers;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Queries;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Commands;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Handlers.Commands;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Handlers.Queries;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Mappers;
using ElTocardo.Application.Mediator.PresetChatOptionsMediator.Queries;
using ElTocardo.Application.Services;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Repositories;
using ElTocardo.Infrastructure.Mappers.Dtos.AI;
using ElTocardo.Infrastructure.Mappers.Dtos.ModelContextProtocol;
using ElTocardo.Infrastructure.Mediator.ApplicationUserMediator;
using ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Commands;
using ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Handlers.Commands;
using ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Handlers.Queries;
using ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Queries;
using ElTocardo.Infrastructure.Mediator.Data;
using ElTocardo.Infrastructure.Mediator.Repositories;
using ElTocardo.Infrastructure.Options;
using ElTocardo.Infrastructure.Services;
using ElTocardo.Infrastructure.Services.Endpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
    public static IServiceCollection AddElTocardoInfrastructure<TApplicationDbContextOptionsConfiguration>(this IServiceCollection services,
        IConfiguration configuration) where TApplicationDbContextOptionsConfiguration : class, IDbContextOptionsConfiguration<ApplicationDbContext>
    {
        return services
            .AddSingleton<IDbContextOptionsConfiguration<ApplicationDbContext>, TApplicationDbContextOptionsConfiguration>()
            .AddOptions(configuration)
            .AddElTocardoApplication(configuration)
            .AddAiClients(configuration)
            .AddDbContext<ApplicationDbContext>()
            .AddValidation()
            .AddMappers()
            .AddServices();
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
                .AddPresetChatOptionsService();
    }

    private static IServiceCollection AddPresetChatOptionsService(this IServiceCollection services)
    {
        services.AddScoped<IPresetChatOptionsRepository, PresetChatOptionsRepository>();


        // PresetChatOptions Mappers
        services.AddSingleton<PresetChatOptionsDomainGetDtoMapper>();
        services.AddSingleton<PresetChatOptionsDomainGetAllDtoMapper>();
        services.AddSingleton<PresetChatOptionsDomainUpdateCommandMapper>();
        services.AddSingleton<PresetChatOptionsDomainCreateCommandMapper>();


        // PresetChatOptions Command handlers
        services
            .AddScoped<ICommandHandler<CreatePresetChatOptionsCommand, Guid>, CreatePresetChatOptionsCommandHandler>();
        services
            .AddScoped<ICommandHandler<UpdatePresetChatOptionsCommand>, UpdatePresetChatOptionsCommandHandler>();
        services
            .AddScoped<ICommandHandler<DeletePresetChatOptionsCommand>, DeletePresetChatOptionsCommandHandler>();

        // PresetChatOptions Query handlers
        services
            .AddScoped<IQueryHandler<GetAllPresetChatOptionsQuery, List<PresetChatOptionsDto>>,
                GetAllPresetChatOptionsQueryHandler>();
        services
            .AddScoped<IQueryHandler<GetPresetChatOptionsByNameQuery, PresetChatOptionsDto>,
                GetPresetChatOptionsByNameQueryHandler>();

        services.AddScoped<IPresetChatOptionsEndpointService, PresetChatOptionsEndpointService>();

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

    private static IServiceCollection AddMcpServerConfigurationService(this IServiceCollection services)
    {
        services.AddScoped<IMcpServerConfigurationRepository, McpServerConfigurationRepository>();


        // MCP Mappers
        services.AddSingleton<McpServerConfigurationDomainGetDtoMapper>();
        services.AddSingleton<McpServerConfigurationDomainGetAllDtoMapper>();
        services.AddSingleton<McpServerConfigurationDomainUpdateCommandMapper>();
        services.AddSingleton<McpServerConfigurationDomainCreateCommandMapper>();


        // MCP Command handlers
        services.AddScoped<ICommandHandler<CreateMcpServerCommand, Guid>, CreateMcpServerCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateMcpServerCommand>, UpdateMcpServerCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteMcpServerCommand>, DeleteMcpServerCommandHandler>();

        // MCP Query handlers
        services
            .AddScoped<IQueryHandler<GetAllMcpServersQuery, Dictionary<string, McpServerConfigurationItemDto>>,
                GetAllMcpServersQueryHandler>();
        services
            .AddScoped<IQueryHandler<GetMcpServerByNameQuery, McpServerConfigurationItemDto>,
                GetMcpServerByNameQueryHandler>();

        services.AddScoped<IMcpServerConfigurationEndpointService, McpServerConfigurationEndpointService>();

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

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateMcpServerCommand>();
        return services;
    }
}
