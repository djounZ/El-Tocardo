using ElTocardo.Authorization.EntityFramework.Infrastructure;
using ElTocardo.Authorization.Server.Configuration.EntityFramework;
using ElTocardo.Authorization.Server.Options;
using ElTocardo.ServiceDefaults;
using Microsoft.AspNetCore.Authentication.Cookies;
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
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddDataProtection()
                .PersistKeysToDbContext<AuthorizationDbContext>()
                .SetApplicationName(ElTocardoDataProtectionApplicationName);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/account/login";  // Path to the login page
                    options.LogoutPath = "/account/logout"; // Path to the logout page
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Duration of the authentication cookie
                    options.SlidingExpiration = true;  // Renews the cookie's expiration time with each request
                    options.Cookie.HttpOnly = true; // Ensures that the cookie is accessible only via HTTP, not JavaScript
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Ensures the cookie is sent over HTTPS only if the request is HTTPS

                });

            services
                .AddSingleton<IDbContextOptionsConfiguration<AuthorizationDbContext>,
                    ApiDbContextOptionsConfiguration>();
            services.AddDbContext<AuthorizationDbContext>();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AuthorizationDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();;

            services.AddOAuth2Oidc();
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
                    //  options.UseAspNetCore().DisableTransportSecurityRequirement();


                    options
                        .AllowClientCredentialsFlow()
                        .AllowPasswordFlow()
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
                    options.RegisterScopes(OpenIddictConstants.Scopes.Email, OpenIddictConstants.Scopes.Profile, OpenIddictConstants.Scopes.Roles);

                    // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                    options
                        .UseAspNetCore()
                        .EnableTokenEndpointPassthrough()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableUserInfoEndpointPassthrough();
                });

            return services;
        }
    }
}
