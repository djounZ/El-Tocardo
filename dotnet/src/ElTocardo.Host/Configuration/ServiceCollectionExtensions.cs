using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElTocardo.Host.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElTocardoHost(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
