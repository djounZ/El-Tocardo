using System.Text.Json;
using AI.GithubCopilot.Configuration;
using ElTocardo.Application.Configuration;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Services;
using ElTocardo.Infrastructure.Mappers.Dtos.AI;
using ElTocardo.Infrastructure.Options;
using ElTocardo.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OllamaSharp;

namespace ElTocardo.Infrastructure.Configuration;

public static class ServiceCollectionExtensions
{


    /// <summary>
    /// Registers  ElTocardo Infrastructure services and options.
    /// Requires AiGithubCopilotUserProvider to be registered in the service collection.
    /// Requires IMemoryCache to be registered in the service collection.
    /// </summary>
    public static IServiceCollection AddElTocardoInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddElTocardoApplication(configuration)
            .AddAiGithubCopilot(configuration)
            .AddOllamaApiClient(configuration)
            .AddOptions(configuration)
            .AddMappers(configuration)
            .AddServices(configuration);
        return services;
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
    private static IServiceCollection AddServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IAiProviderService, AiProviderService>();
        services.AddTransient<ChatClientStore>();
        services.AddTransient<ChatClientProvider>();
        services.AddTransient<IChatCompletionsService, ChatCompletionsService>();
        AddMcpServerConfigurationDto(services);
        services.AddTransient<IMcpServerConfigurationProviderService, McpServerConfigurationProviderService>();

        return services;
    }

    private static void AddMcpServerConfigurationDto(IServiceCollection services)
    {
        services.AddTransient(sc =>
        {
            var requiredService = sc.GetRequiredService<IOptions<ElTocardoInfrastructureOptions>>();
            if (!File.Exists(requiredService.Value.McpServerConfigurationFile))
            {
                return new McpServerConfigurationDto(new Dictionary<string, McpServerConfigurationItemDto>());
            }

            using var stream = File.OpenRead(requiredService.Value.McpServerConfigurationFile);
            var mcpServerConfiguration = JsonSerializer.Deserialize<McpServerConfigurationDto>(stream)
                                         ?? new McpServerConfigurationDto(
                                             new Dictionary<string, McpServerConfigurationItemDto>());
            return mcpServerConfiguration;
        });
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
