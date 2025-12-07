
using ElTocardo.Authorization.Server.Options;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;

namespace ElTocardo.Authorization.Server.Endpoints;

/// <summary>
/// Minimal API endpoints for OpenIddict authorization, token, and userinfo.
/// </summary>
public static class AuthorizationEndpoints
{
	private static string Tags => "Authorization";

	public static IEndpointRouteBuilder MapAuthorizationEndpoints(this IEndpointRouteBuilder app)
	{
		var options = app.ServiceProvider.GetRequiredService<IOptions<ElTocardoAuthorizationServerOptions>>().Value.OpenIddictServerOptions;

		// /connect/authorize endpoint (Authorization Code flow)
		app.MapGet(options.AuthorizationEndpointUri, async (
			HttpContext context
		) =>
		{
			var request = context.GetOpenIddictServerRequest() ??
						  throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

			AuthenticateResult result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			if (!result.Succeeded)
			{
				return Results.Challenge(
					authenticationSchemes: [CookieAuthenticationDefaults.AuthenticationScheme],
					properties: new AuthenticationProperties
					{
						RedirectUri = context.Request.PathBase + context.Request.Path + QueryString.Create(
							context.Request.HasFormContentType ? context.Request.Form.ToList() : context.Request.Query.ToList())
					});
			}

			var claims = new List<Claim>
			{
				new (OpenIddictConstants.Claims.Subject, result?.Principal?.Identity?.Name ?? string.Empty)
			};
			var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			claimsPrincipal.SetScopes(request.GetScopes());
			return Results.SignIn(claimsPrincipal, properties: null, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
		})
		.WithName("Authorize")
		.WithTags(Tags)
        .AddOpenApiOperationTransformer((operation, context, ct) =>
        {
            // Per-endpoint tweaks
            operation.Summary = "Gets the current weather report.";
            operation.Description = "Returns a short description and emoji.";
            return Task.CompletedTask;
        });


        // /connect/token endpoint (all grant types)
        app.MapPost(options.TokenEndpointUri, async (
			HttpContext context,
			UserManager<IdentityUser> userManager
		) =>
		{
			var request = context.GetOpenIddictServerRequest() ??
						  throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
			ClaimsPrincipal claimsPrincipal;

			if (request.IsPasswordGrantType())
			{
				var user = await userManager.FindByNameAsync(request.Username ?? string.Empty);
				if (user == null || !await userManager.CheckPasswordAsync(user, request.Password ?? string.Empty))
				{
					return Results.Forbid(authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
				}
				var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, OpenIddictConstants.Claims.Name, OpenIddictConstants.Claims.Role);
				identity.AddClaim(new Claim(OpenIddictConstants.Claims.Subject, user.Id.ToString())
					.SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken));
				identity.AddClaim(new Claim(OpenIddictConstants.Claims.Email, user.Email ?? string.Empty)
					.SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken));
				// Optionally add audience claim if needed for your API
				var roles = await userManager.GetRolesAsync(user);
				foreach (var role in roles)
				{
					identity.AddClaim(new Claim(ClaimTypes.Role, role)
						.SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken));
				}
				// Optionally add name claim if needed
				claimsPrincipal = new ClaimsPrincipal(identity);
				claimsPrincipal.SetScopes(request.GetScopes());
			}
			else if (request.IsClientCredentialsGrantType())
			{
				var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
				identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId ?? throw new InvalidOperationException());
				// Optionally add custom claims for client credentials flow
				claimsPrincipal = new ClaimsPrincipal(identity);
				claimsPrincipal.SetScopes(request.GetScopes());
			}
			else if (request.IsAuthorizationCodeGrantType())
			{
				claimsPrincipal = (await context.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme))?.Principal ?? new ClaimsPrincipal();
			}
			else if (request.IsRefreshTokenGrantType())
			{
				claimsPrincipal = (await context.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme))?.Principal ?? new ClaimsPrincipal();
			}
			else
			{
				return Results.BadRequest(new ProblemDetails { Title = "Login failed", Detail = "The specified grant type is not implemented." });
			}
			return Results.SignIn(claimsPrincipal, properties: null, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
		})
		.WithName("Token")
		.WithTags(Tags)
        .AddOpenApiOperationTransformer((operation, context, ct) =>
        {
            // Per-endpoint tweaks
            operation.Summary = "Gets the current weather report.";
            operation.Description = "Returns a short description and emoji.";
            return Task.CompletedTask;
        });

        // /connect/userinfo endpoint
        app.MapGet(options.UserInfoEndpointUri, async (
			HttpContext context
		) =>
		{
			var claimsPrincipal = (await context.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
			return Results.Ok(new
			{
				Name = claimsPrincipal?.GetClaim(OpenIddictConstants.Claims.Subject),
				Occupation = "Developer",
				Age = 43
			});
		})
		.WithName("Userinfo")
		.WithTags(Tags)
        .AddOpenApiOperationTransformer((operation, context, ct) =>
        {
            // Per-endpoint tweaks
            operation.Summary = "Gets the current weather report.";
            operation.Description = "Returns a short description and emoji.";
            return Task.CompletedTask;
        });

        return app;
	}
}
