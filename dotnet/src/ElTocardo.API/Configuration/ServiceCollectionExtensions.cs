using AI.GithubCopilot.Infrastructure.Services;
using ElTocardo.API.Configuration.EntityFramework;
using ElTocardo.API.Options;
using ElTocardo.Infrastructure.Configuration;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenIddict.Validation.AspNetCore;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace ElTocardo.API.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElTocardoApi(this IServiceCollection services, IConfiguration configuration, string applicationName)
    {
        services.AddServiceDiscovery();
        services.ConfigureHttpClientDefaults(http =>
        {
            if (http.Name != nameof(GithubCopilotChatCompletion))
            {
                // Turn on resilience by default
                http.AddStandardResilienceHandler();
            }

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        services.Configure<ElTocardoApiOptions>(configuration.GetSection(nameof(ElTocardoApiOptions)));
        services.AddHttpContextAccessor();
        services.AddCaching()
            .AddSingleton<AiGithubCopilotUserProvider>(sc =>
        {
            return new AiGithubCopilotUserProvider(() =>
                sc.GetRequiredService<IHttpContextAccessor>().HttpContext?.User.Identity?.Name ?? string.Empty);
        });
        var mongoClientSettings = MongoClientSettings.FromConnectionString(configuration.GetConnectionString("el-tocardo-db-mongodb"));

        services
            .AddElTocardoInfrastructure<ApiDbContextOptionsConfiguration>(configuration, mongoClientSettings,"el-tocardo-db-mongodb");

        services.AddOpenTelemetryExporters(configuration)
            .AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(applicationName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            });
        services.AddOAuth2Oidc(configuration);
        return services;
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
              options.SetIssuer(configuration["OpenIddictIssuer"]!);
              options.UseIntrospection()
                  .SetClientId("postman")
                  .SetClientSecret("postman-secret");

              options.UseSystemNetHttp();
              options.UseAspNetCore();
          });
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            });
        services.AddAuthorization();

        return services;
    }
    private static IServiceCollection AddOpenTelemetryExporters(this IServiceCollection services, IConfiguration configuration)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
        if (useOtlpExporter)
        {
            services.AddOpenTelemetry().UseOtlpExporter();
        }

        return services;
    }



}
