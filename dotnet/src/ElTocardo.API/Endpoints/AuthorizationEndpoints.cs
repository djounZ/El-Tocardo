using ElTocardo.API.Options;
using ElTocardo.Infrastructure.Mediator.ApplicationUserMediator;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace ElTocardo.API.Endpoints;

/// <summary>
/// Minimal API endpoints for user registration, login, password reset, and unregistration.
/// </summary>
public static class AuthorizationEndpoints
{
    private static string Tags => "Authorization";

    public static IEndpointRouteBuilder MapAuthorizationEndpoints(this IEndpointRouteBuilder app)
    {
        var options = app.ServiceProvider.GetRequiredService<IOptions<ElTocardoApiOptions>>().Value.OpenIddictServerOptions;

        app.MapPost(options.TokenEndpointUri, async (
                HttpContext context,
                UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager) =>
        {


            var request = context.GetOpenIddictServerRequest()!;


            if (request.IsPasswordGrantType())
            {
                var user = await userManager.FindByNameAsync(request.Username!);
                if (user == null)
                {
                    return Results.Forbid(
                        authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Invalid credentials."
                        }!));
                }

                var result = await signInManager.CheckPasswordSignInAsync(user, request.Password!, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    return Results.Forbid(
                        authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Invalid credentials."
                        }!));
                }

                var principal = await signInManager.CreateUserPrincipalAsync(user);
                principal.SetScopes(OpenIddictConstants.Scopes.OpenId, OpenIddictConstants.Scopes.Profile);

                return Results.SignIn(principal, properties: null, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            return Results.BadRequest(new ProblemDetails { Title = "Login failed", Detail = "The specified grant type is not implemented." });
        })
        .WithName("Token")
        .WithTags(Tags)
        .WithOpenApi();


        return app;
    }
}
