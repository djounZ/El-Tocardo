
using ElTocardo.Authorization.Server.Options;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using System.Text.Json;

namespace ElTocardo.Authorization.Server.Endpoints;

/// <summary>
/// Minimal API endpoints for OpenIddict authorization, token, and userinfo.
/// </summary>
public static class ConnectEndpoints
{
	private static string Tags => "Connect";

	public static IEndpointRouteBuilder MapConnectEndpoints(this IEndpointRouteBuilder app)
	{
		var options = app.ServiceProvider.GetRequiredService<IOptions<ElTocardoAuthorizationServerOptions>>().Value.OpenIddictServerOptions;

		// /connect/authorize endpoint (Authorization Code flow)
		app.MapMethods(options.AuthorizationEndpointUri, [HttpMethods.Get, HttpMethods.Post], async (HttpContext context, IOpenIddictScopeManager manager
		) =>
		{
			var request = context.GetOpenIddictServerRequest() ??
						  throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

			AuthenticateResult result = await context.AuthenticateAsync();
			if (!result.Succeeded)
			{
				return Results.Challenge(
					authenticationSchemes: [IdentityConstants.ApplicationScheme],
					properties: new AuthenticationProperties
					{
						RedirectUri = context.Request.PathBase + context.Request.Path + QueryString.Create(
							context.Request.HasFormContentType ? context.Request.Form.ToList() : context.Request.Query.ToList()),
                Items = { 
                    ["openiddict_request"] = JsonSerializer.Serialize(request) // Store full request
                }
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
		.WithTags(Tags);


  //       // /connect/userinfo endpoint
  //       app.MapGet(options.UserInfoEndpointUri, async (
		// 	HttpContext context
		// ) =>
		// {
		// 	var claimsPrincipal = (await context.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
		// 	return Results.Ok(new
		// 	{
		// 		Name = claimsPrincipal?.GetClaim(OpenIddictConstants.Claims.Subject),
		// 		Occupation = "Developer",
		// 		Age = 43
		// 	});
		// })
		// .WithName("Userinfo")
		// .WithTags(Tags);

        return app;
	}
}
