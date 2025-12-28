using ElTocardo.Infrastructure.EntityFramework.Configuration;
using ElTocardo.Migrations.Options;
using Microsoft.AspNetCore.Identity;
using static ElTocardo.ServiceDefaults.Constants;

namespace ElTocardo.Migrations.Configuration.EntityFramework;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddElTocardoMigrations(IConfiguration configuration)
        {
            services.Configure<ElTocardoMigrationsOptions>(configuration.GetSection(nameof(ElTocardoMigrationsOptions)));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<MigrationDbContext>()
                .AddDefaultTokenProviders();
            return services.AddElTocardoInfrastructureEntityFramework<EntityFrameworkDbContextOptionsConfiguration, MigrationDbContext>(ElTocardoDataProtectionApplicationName)
                .AddOpenIddict();

        }

        private IServiceCollection AddOpenIddict()
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
}
