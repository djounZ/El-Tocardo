using ElTocardo.Domain.Configuration;

namespace ElTocardo.Application.Configuration;

public static class ServiceProviderExtensions
{
    public static async Task UseElTocardoApplicationAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        await serviceProvider.UseElTocardoDomainAsync(cancellationToken);
    }

}
