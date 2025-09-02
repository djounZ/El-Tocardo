using System.Security.Claims;
using ElTocardo.API.Options;
using ElTocardo.Infrastructure.EntityFramework.Mediator.ApplicationUserMediator;
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
// Ensure the 'sub' claim is present
                if (!principal.HasClaim(c => c.Type == OpenIddictConstants.Claims.Subject))
                {
                    var identity = (ClaimsIdentity)principal.Identity!;
                    identity.AddClaim(new Claim(OpenIddictConstants.Claims.Subject, user.Id));
                }
                principal.SetScopes(request.GetScopes());

                return Results.SignIn(principal, properties: null, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }



            // Refresh token grant type
            if (request.IsRefreshTokenGrantType())
            {
                // Authenticate the refresh token and retrieve the user principal
                var authenticateResult = await context.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                var principal = authenticateResult.Principal;

                if (principal == null)
                {
                    return Results.Forbid(
                        authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The refresh token is invalid or has expired."
                        }!));
                }

                // Optionally, you can revalidate the user (e.g., check if still active)
                var userId = principal.GetClaim(OpenIddictConstants.Claims.Subject);
                var user = userId != null ? await userManager.FindByIdAsync(userId) : null;
                if (user == null || !await userManager.IsEmailConfirmedAsync(user))
                {
                    return Results.Forbid(
                        authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
                        }!));
                }

                // Optionally, update claims or scopes here

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
