namespace ElTocardo.Host.Configuration;

public static class ServiceProviderExtensions
{
    public static async Task UseElTocardoHostAsync(this IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
