using Microsoft.AspNetCore.Builder;

namespace ElTocardo.Infrastructure.Configuration;

public static class ApplicationBuilderExtensions
{
    public static async Task UseElTocardoInfrastructureAsync(this IApplicationBuilder app, CancellationToken cancellationToken)
    {
        await app.ApplicationServices.UseElTocardoInfrastructureAsync(cancellationToken);
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
