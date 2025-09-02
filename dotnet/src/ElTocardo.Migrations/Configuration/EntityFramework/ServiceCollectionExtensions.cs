using ElTocardo.Infrastructure.EntityFramework.Configuration;
using ElTocardo.Infrastructure.EntityFramework.Mediator.Data;
using ElTocardo.Migrations.Options;

namespace ElTocardo.Migrations.Configuration.EntityFramework;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElTocardoMigrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ElTocardoMigrationsOptions>(configuration.GetSection(nameof(ElTocardoMigrationsOptions)));

        return services.AddElTocardoInfrastructureEntityFramework<EntityFrameworkDbContextOptionsConfiguration>()
            .AddOpenIddict();

    }



    private static IServiceCollection AddOpenIddict(this IServiceCollection services)
    {
        OpenIddictExtensions.AddOpenIddict(services)
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<ApplicationDbContext>();
            });
        return services;
    }



}
