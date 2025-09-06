using ElTocardo.Infrastructure.EntityFramework.Configuration;
using ElTocardo.Infrastructure.EntityFramework.Mediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Data;
using ElTocardo.Migrations.Options;
using Microsoft.AspNetCore.Identity;

namespace ElTocardo.Migrations.Configuration.EntityFramework;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElTocardoMigrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ElTocardoMigrationsOptions>(configuration.GetSection(nameof(ElTocardoMigrationsOptions)));
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<MigrationDbContext>()
            .AddDefaultTokenProviders();
        return services.AddElTocardoInfrastructureEntityFramework<EntityFrameworkDbContextOptionsConfiguration, MigrationDbContext>()
            .AddOpenIddict();

    }



    private static IServiceCollection AddOpenIddict(this IServiceCollection services)
    {
        OpenIddictExtensions.AddOpenIddict(services)
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<MigrationDbContext>();
            });
        return services;
    }



}
