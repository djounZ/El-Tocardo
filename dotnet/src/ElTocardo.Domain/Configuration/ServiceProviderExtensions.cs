namespace ElTocardo.Domain.Configuration;

public static class ServiceProviderExtensions
{
    public static async Task UseElTocardoDomainAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

}
