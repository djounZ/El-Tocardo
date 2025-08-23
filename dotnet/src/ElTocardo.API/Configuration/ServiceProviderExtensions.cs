using ElTocardo.Infrastructure.Configuration;

namespace ElTocardo.API.Configuration;

public static class ServiceProviderExtensions
{
    internal static async Task UseElTocardoApiAsync(this IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await serviceProvider.UseElTocardoInfrastructureAsync(cancellationToken);
    }
}
