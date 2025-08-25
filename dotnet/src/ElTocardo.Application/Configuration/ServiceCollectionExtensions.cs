using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Dtos.Conversation;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.ConversationMediator.Commands;
using ElTocardo.Application.Mediator.ConversationMediator.Handlers.Commands;
using ElTocardo.Application.Mediator.ConversationMediator.Handlers.Queries;
using ElTocardo.Application.Mediator.ConversationMediator.Mappers;
using ElTocardo.Application.Mediator.ConversationMediator.Queries;
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
using ElTocardo.Application.Options;
using ElTocardo.Application.Services;
using ElTocardo.Domain.Configuration;
using ElTocardo.Domain.Mediator.ConversationMediator.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElTocardo.Application.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElTocardoApplication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ElTocardoApplicationOptions>(configuration.GetSection(nameof(ElTocardoApplicationOptions)));
        return services
            .AddElTocardoDomain(configuration)
            .AddMcpServerConfigurationService()
            .AddPresetChatOptionsService()
            .AddConversationService()
            ;
    }



    private static IServiceCollection AddMcpServerConfigurationService(this IServiceCollection services)
    {

        // MCP Mappers
        services.AddSingleton<McpServerConfigurationDomainGetDtoMapper>();
        services.AddSingleton<McpServerConfigurationDomainGetAllDtoMapper>();
        services.AddSingleton<McpServerConfigurationDomainUpdateCommandMapper>();
        services.AddSingleton<McpServerConfigurationDomainCreateCommandMapper>();


        // MCP Command handlers
        services.AddScoped<ICommandHandler<CreateMcpServerCommand, Guid>, CreateMcpServerCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateMcpServerCommand>, UpdateMcpServerCommandByKeyHandler>();
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

    private static IServiceCollection AddPresetChatOptionsService(this IServiceCollection services)
    {

        // PresetChatOptions Mappers
        services.AddSingleton<PresetChatOptionsDomainGetDtoMapper>();
        services.AddSingleton<PresetChatOptionsDomainGetAllDtoMapper>();
        services.AddSingleton<PresetChatOptionsDomainUpdateCommandMapper>();
        services.AddSingleton<PresetChatOptionsDomainCreateCommandMapper>();


        // PresetChatOptions Command handlers
        services
            .AddScoped<ICommandHandler<CreatePresetChatOptionsCommand, Guid>, CreatePresetChatOptionsCommandHandler>();
        services
            .AddScoped<ICommandHandler<UpdatePresetChatOptionsCommand>, UpdatePresetChatOptionsCommandByKeyHandler>();
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




    private static IServiceCollection AddConversationService(this IServiceCollection services)
    {


        //  Mappers
        services.AddSingleton<ConversationDomainCreateCommandMapper>();
        services.AddSingleton<ConversationDomainGetAllDtoMapper>();
        services.AddSingleton<ConversationDomainGetDtoMapper>();
        services.AddSingleton<ConversationDomainUpdateChatResponseCommandMapper>();
        services.AddSingleton<ConversationDomainUpdateUserMessageCommandMapper>();


        //  Command handlers
        services.AddScoped<ICommandHandler<CreateConversationCommand, string>, CreateConversationCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteConversationCommand>, DeleteConversationCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateConversationUpdateRoundCommand,Conversation>, UpdateConversationUpdateRoundCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateConversationAddNewRoundCommand,Conversation>, UpdateConversationAddNewRoundCommandHandler>();

        //  Query handlers
        services
            .AddScoped<IQueryHandler<GetAllConversationsQuery, ConversationResponseDto[]>,
                GetAllConversationsQueryHandler>();
        services
            .AddScoped<IQueryHandler<GetConversationByIdQuery, ConversationResponseDto>,
                GetConversationByIdQueryHandler>();


        return services;
    }
}
