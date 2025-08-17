using AI.GithubCopilot.Infrastructure.Services;
using ElTocardo.API.Options;
using ElTocardo.Infrastructure.Configuration;
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
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        services.Configure<ElTocardoApiOptions>(configuration.GetSection(nameof(ElTocardoApiOptions)));
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddSingleton<AiGithubCopilotUserProvider>(sc =>
        {
            return new AiGithubCopilotUserProvider(() =>
                sc.GetRequiredService<IHttpContextAccessor>().HttpContext?.User.Identity?.Name ?? string.Empty);
        });
        services.AddElTocardoInfrastructure(configuration);
        services
            .AddOpenTelemetryExporters(configuration)
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
