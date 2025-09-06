using ElTocardo.Authorization.Server.Configuration.EntityFramework;
using ElTocardo.Authorization.Server.Options;
using ElTocardo.Infrastructure.EntityFramework.Mediator.ApplicationUserMediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;

namespace ElTocardo.Authorization.Server.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElTocardoAuthorizationServer(this IServiceCollection services, IConfiguration configuration)
    {


        services.Configure<ElTocardoAuthorizationServerOptions>(
            configuration.GetSection(nameof(ElTocardoAuthorizationServerOptions)));
        // Add services to the container.
        services.AddControllersWithViews();
        services.AddRazorPages();

        services.AddDataProtection()
            .PersistKeysToDbContext<AuthorizationDbContext>()
            .SetApplicationName("AuthServerApp") // more instance
            .UseCryptographicAlgorithms(new Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel.AuthenticatedEncryptorConfiguration()
            {
                EncryptionAlgorithm = Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.EncryptionAlgorithm.AES_256_CBC,
                ValidationAlgorithm = Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ValidationAlgorithm.HMACSHA256
            });

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

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AuthorizationDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();;

        services.AddOAuth2Oidc();
        return services;
    }





    private static IServiceCollection AddOAuth2Oidc(this IServiceCollection services)
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
                    options
                        .AddEphemeralEncryptionKey()
                        .AddEphemeralSigningKey()
                        .DisableAccessTokenEncryption();

                    // Register scopes (permissions)
                    options.RegisterScopes(OpenIddictConstants.Scopes.Email, OpenIddictConstants.Scopes.Profile, "roles", "popelar-api");

                    // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                    options
                            .UseAspNetCore()
                            .EnableTokenEndpointPassthrough()
                            .EnableAuthorizationEndpointPassthrough()
                            .EnableUserInfoEndpointPassthrough();

                    options.AddDevelopmentEncryptionCertificate()
                            .AddDevelopmentSigningCertificate();
                }).AddValidation(options =>
                {
                    // Import the configuration from the local OpenIddict server instance.
                    options.UseLocalServer();

                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
                });

        return services;
    }

}
