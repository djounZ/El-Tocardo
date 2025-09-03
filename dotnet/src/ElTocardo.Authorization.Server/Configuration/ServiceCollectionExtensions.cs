using ElTocardo.Authorization.Server.Configuration.EntityFramework;
using ElTocardo.Authorization.Server.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;

namespace ElTocardo.Authorization.Server.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElTocardoAuthorizationServer(this IServiceCollection services, IConfiguration configuration)
    {


        services
            .AddSingleton<IDbContextOptionsConfiguration<DbContext>,
                ApiDbContextOptionsConfiguration>();

        services.AddDbContext<DbContext>();

        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<DbContext>()
            .AddDefaultTokenProviders();

        return services;
    }





    private static IServiceCollection AddOAuth2Oidc(this IServiceCollection services)
    {
        services.AddSingleton<IConfigureOptions<OpenIddict.Server.OpenIddictServerOptions>, ConfigureOpenIddictServerOptions>();
        services.AddOpenIddict()
          .AddCore(options =>
          {
              options.UseEntityFrameworkCore()
                  .UseDbContext<DbContext>();
          })
          .AddServer(options =>
          {
              options.AllowPasswordFlow()
                  .AllowRefreshTokenFlow()
                  .AllowClientCredentialsFlow();


              // Environment-specific certificate configuration
              var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
              if (environment == "Development")
              {
                  // Development certificates (auto-generated)
                  options.AddDevelopmentEncryptionCertificate()
                      .AddDevelopmentSigningCertificate();
              }
              else
              {
                  // Production certificates
                  // var signingCertPath = configuration["OpenIddict:SigningCertificate:Path"];
                  // var signingCertPassword = configuration["OpenIddict:SigningCertificate:Password"];
                  // var encryptionCertPath = configuration["OpenIddict:EncryptionCertificate:Path"];
                  // var encryptionCertPassword = configuration["OpenIddict:EncryptionCertificate:Password"];
                  //
                  // if (!string.IsNullOrEmpty(signingCertPath))
                  // {
                  //     options.AddSigningCertificate(signingCertPath, signingCertPassword);
                  // }
                  //
                  // if (!string.IsNullOrEmpty(encryptionCertPath))
                  // {
                  //     options.AddEncryptionCertificate(encryptionCertPath, encryptionCertPassword);
                  // }

                  // Alternative: Use certificates from store
                  // options.AddSigningCertificate("thumbprint");
                  // options.AddEncryptionCertificate("thumbprint");
              }
              options.UseAspNetCore()
                  .EnableTokenEndpointPassthrough()
                  .EnableAuthorizationEndpointPassthrough()
                  .EnableEndSessionEndpointPassthrough()
                  .EnableUserInfoEndpointPassthrough();
          });

        return services;
    }

}
