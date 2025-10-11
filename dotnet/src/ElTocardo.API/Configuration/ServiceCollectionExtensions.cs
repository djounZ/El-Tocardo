using System.Data.Common;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using AI.GithubCopilot.Infrastructure.Services;
using ElTocardo.API.Configuration.EntityFramework;
using ElTocardo.API.Options;
using ElTocardo.API.ToMigrate;
using ElTocardo.Infrastructure.Configuration;
using ElTocardo.Infrastructure.EntityFramework.Mediator;
using ElTocardo.ServiceDefaults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenIddict.Validation.AspNetCore;
using static ElTocardo.ServiceDefaults.Constants;

namespace ElTocardo.API.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElTocardoApi(this IServiceCollection services, IConfiguration configuration, string applicationName)
    {
        services.AddToMigrate();

        services.Configure<ElTocardoApiOptions>(configuration.GetSection(nameof(ElTocardoApiOptions)));
        services.AddHttpContextAccessor();
        services.AddCaching();
        var mongoClientSettings = MongoClientSettings.FromConnectionString(configuration.GetConnectionString(MongoDbDatabaseResourceName));


        return services
            .AddElTocardoInfrastructure<ApiDbContextOptionsConfiguration, GithubCopilotAccessTokenResponseDtoProvider, GithubAccessTokenStore>(
                configuration,
                mongoClientSettings,
                MongoDbDatabaseResourceName,
                ElTocardoDataProtectionApplicationName)
            .AddOAuth2Oidc(configuration);
    }

    private static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddDistributedMemoryCache(); // For in-memory session storage
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        services.AddOutputCache();
        services.AddSingleton<IConfigureOptions<OutputCacheOptions>, OutputCacheOptionsSetup>();
        return services;
    }



    private static IServiceCollection AddOAuth2Oidc(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddOpenIddict()
          .AddValidation(options =>
          {
              options.SetIssuer(configuration[OpenIddictIssuerEnvironmentVariableName]!);
              options.UseSystemNetHttp();
              options.UseAspNetCore();
              options.HandleEncryptionCertificate(services);
          });
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            });
        services.AddAuthorization();

        return services;
    }

    private static void HandleEncryptionCertificate(this OpenIddictValidationBuilder options, IServiceCollection services)
    {
        // Encryption and signing of tokens
        // Load persistent certificate from the database
        using var scope = services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.OpenConnection();
        using var command = dbContext.Database.GetDbConnection().CreateCommand();
        var cert = command.LoadFromDataBaseX509Certificate2(OpenIddictServerCertificateName);

        if (cert is not null)
        {
            options.AddEncryptionCertificate(cert);
            options.AddSigningCertificate(cert);
        }
    }



    private static IServiceCollection AddToMigrate(this IServiceCollection services)
    {
        services.TryAddTransient<HttpListener>();
        services.TryAddTransient<TaskCompletionSource<bool>>();
        services.TryAddSingleton<HttpClientRunner>();
        services.TryAddTransient<GithubAuthenticator>();
        services.TryAddTransient<GithubAccessTokenProvider>();
        return services;
    }

}
