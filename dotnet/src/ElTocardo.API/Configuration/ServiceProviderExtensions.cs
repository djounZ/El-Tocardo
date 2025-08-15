using ElTocardo.Infrastructure.Configuration;

namespace ElTocardo.API.Configuration;

public static class ServiceProviderExtensions
{
    public static async Task UseElTocardoApiAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        await serviceProvider.UseAddElTocardoInfrastructureAsync(cancellationToken);
    }

}
