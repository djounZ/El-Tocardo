using ElTocardo.Authorization.EntityFramework.Infrastructure;
using ElTocardo.Authorization.Server.Configuration.EntityFramework;
using ElTocardo.Authorization.Server.Handlers;
using ElTocardo.Authorization.Server.Options;
using ElTocardo.ServiceDefaults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using static ElTocardo.ServiceDefaults.Constants;

namespace ElTocardo.Authorization.Server.Configuration;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddElTocardoAuthorizationServer(IConfiguration configuration)
        {


            services.Configure<ElTocardoAuthorizationServerOptions>(
                configuration.GetSection(nameof(ElTocardoAuthorizationServerOptions)));
            // Add services to the container.
            services.AddDataProtection()
                .PersistKeysToDbContext<AuthorizationDbContext>()
                .SetApplicationName(ElTocardoDataProtectionApplicationName);

            services
                .AddSingleton<IDbContextOptionsConfiguration<AuthorizationDbContext>,
                    ApiDbContextOptionsConfiguration>();
            services.AddDbContext<AuthorizationDbContext>();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AuthorizationDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();;
            //
            // services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
            //     .AddEntityFrameworkStores<AuthorizationDbContext>();

            services.AddOAuth2Oidc();
            services.AddRazorPages();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.SetIsOriginAllowed(_ => true)  // Allows any origin
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();  // Required for auth cookies/tokens
                });
            });
            return services;
        }

        private IServiceCollection AddOAuth2Oidc()
        {
            services.AddSingleton<IConfigureOptions<OpenIddict.Server.OpenIddictServerOptions>, ConfigureOpenIddictServerOptions>();
            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                        .UseDbContext<AuthorizationDbContext>();
                })

                // Register the OpenIddict server components.
                .AddServer(options =>
                {
                    options.AddEventHandler(CustomTokenRequestHandler.Descriptor);
                    options.AddEventHandler(CustomEndSessionHandler.Descriptor);

                    options
                        .AllowClientCredentialsFlow()
                        .AllowAuthorizationCodeFlow()
                        .RequireProofKeyForCodeExchange()
                        .AllowRefreshTokenFlow();

                    // Encryption and signing of tokens
                    // Load persistent certificate from the database
                    using var scope = services.BuildServiceProvider().CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<AuthorizationDbContext>();

                    dbContext.Database.OpenConnection();
                    using var command = dbContext.Database.GetDbConnection().CreateCommand();
                    var cert = command.LoadFromDataBaseX509Certificate2(OpenIddictServerCertificateName);

                    if (cert is not null)
                    {
                        options.AddEncryptionCertificate(cert);
                        options.AddSigningCertificate(cert);
                    }


                    // Register scopes (permissions)
                    options.RegisterScopes(OpenIddictConstants.Scopes.Email, OpenIddictConstants.Scopes.Profile, OpenIddictConstants.Scopes.Roles, OpenIddictElTocardoApiUserScope);

                    // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                    var openIddictServerAspNetCoreBuilder = options
                        .UseAspNetCore()
                        .EnableAuthorizationEndpointPassthrough();


                    openIddictServerAspNetCoreBuilder
                        .DisableTransportSecurityRequirement();

                });

            return services;
        }
    }
}
