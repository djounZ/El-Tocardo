using AI.GithubCopilot.Infrastructure.Services;
using ElTocardo.API.Options;
using ElTocardo.Infrastructure.Configuration;

namespace ElTocardo.API.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElTocardoApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ElTocardoApiOptions>(configuration.GetSection(nameof(ElTocardoApiOptions)));
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddSingleton<AiGithubCopilotUserProvider>(sc =>
        {
            return new AiGithubCopilotUserProvider(() =>
                sc.GetRequiredService<IHttpContextAccessor>().HttpContext?.User.Identity?.Name ?? string.Empty);
        });
        services.AddElTocardoInfrastructure(configuration);
        return services;
    }
}
