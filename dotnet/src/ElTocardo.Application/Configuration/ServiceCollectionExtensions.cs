using ElTocardo.Application.Options;
using ElTocardo.Domain.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElTocardo.Application.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElTocardoApplication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ElTocardoApplicationOptions>(configuration.GetSection(nameof(ElTocardoApplicationOptions)));
        services.AddElTocardoDomain(configuration);
        return services;
    }
}
