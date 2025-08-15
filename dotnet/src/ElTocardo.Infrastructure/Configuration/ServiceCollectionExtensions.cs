using AI.GithubCopilot.Configuration;
using ElTocardo.Application.Configuration;
using ElTocardo.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElTocardo.Infrastructure.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElTocardoInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ElTocardoInfrastructureOptions>(configuration.GetSection(nameof(ElTocardoInfrastructureOptions)));
        services.AddAiGithubCopilot(configuration);
        services.AddElTocardoApplication(configuration);
        return services;
    }
}
