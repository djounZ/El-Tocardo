using ElTocardo.Domain.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElTocardo.Domain.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElTocardoDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ElTocardoDomainOptions>(configuration.GetSection(nameof(ElTocardoDomainOptions)));
        return services;
    }
}
