using System.Net;
using AI.GithubCopilot.Infrastructure.Services;
using ElTocardo.API.Configuration.Authorization;
using ElTocardo.API.Configuration.EntityFramework;
using ElTocardo.API.Options;
using ElTocardo.API.ToMigrate;
using ElTocardo.Infrastructure.Configuration;
using ElTocardo.Infrastructure.EntityFramework.Mediator;
using ElTocardo.ServiceDefaults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using MongoDB.Driver;
using OpenIddict.Validation.AspNetCore;
using static ElTocardo.ServiceDefaults.Constants;

namespace ElTocardo.API.Configuration;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddElTocardoApi(IConfiguration configuration, string applicationName)
        {
            services.AddToMigrate()

                .Configure<ElTocardoApiOptions>(configuration.GetSection(nameof(ElTocardoApiOptions)));
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

        private IServiceCollection AddCaching()
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

        private IServiceCollection AddOAuth2Oidc(IConfiguration configuration)
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

            services.AddScoped<IAuthorizationHandler, ScopeHandler>();
            services.AddAuthorization(options =>
            {
                // Default policy: ALL endpoints require toto scope
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new ScopeRequirement(OpenIddictElTocardoApiUserScope))
                    .Build();

                // Explicit "allow all" policy for public endpoints
                options.AddPolicy("AllowAnonymous", policy => policy.RequireAssertion(_ => true));
            });
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, ct) =>
                {
                    document.Components ??= new();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            ClientCredentials = new OpenApiOAuthFlow
                            {
                                TokenUrl = new Uri(configuration[OpenIddictIssuerEnvironmentVariableName]!),
                                Scopes = new Dictionary<string, string>
                                {
                                    [OpenIddictElTocardoApiUserScope] = "Access required"
                                }
                            }
                        }
                    };

                    // ✅ CORRECT: Pass scheme reference as constructor parameter
                    document.Security = new List<OpenApiSecurityRequirement>
                    {
                        new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecuritySchemeReference("Bearer"),  // ✅ Just the string ID
                                new List<string> { OpenIddictElTocardoApiUserScope }
                            }
                        }
                    };

                    return Task.CompletedTask;
                });
            });

            return services;
        }
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
