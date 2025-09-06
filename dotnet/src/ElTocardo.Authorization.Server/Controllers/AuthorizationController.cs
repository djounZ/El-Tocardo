using System.Security.Claims;
using ElTocardo.Infrastructure.EntityFramework.Mediator.ApplicationUserMediator;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace ElTocardo.Authorization.Server.Controllers
{
    public class AuthorizationController(UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                          throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // Retrieve the user principal stored in the authentication cookie.
            AuthenticateResult result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // If the user principal can't be extracted, redirect the user to the login page.
            if (!result.Succeeded)
            {
                return Challenge(
                    authenticationSchemes: CookieAuthenticationDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                            Request.HasFormContentType ? [.. Request.Form] : Request.Query.ToList())
                    });
            }

            // Create a new claims principal
            var claims = new List<Claim>
            {
                // 'subject' claim which is required
                new (OpenIddictConstants.Claims.Subject, result?.Principal?.Identity?.Name??string.Empty),
                new Claim("some claim", "some value").SetDestinations(OpenIddictConstants.Destinations.AccessToken),
                new Claim(OpenIddictConstants.Claims.Email, "some@email").SetDestinations(OpenIddictConstants.Destinations.IdentityToken)
            };

            var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Set requested scopes (this is not done automatically)
            claimsPrincipal.SetScopes(request.GetScopes());

            // Signing in with the OpenIddict authentiction scheme trigger OpenIddict to issue a code (which can be exchanged for an access token)
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpPost("~/connect/token")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                          throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            ClaimsPrincipal claimsPrincipal;

            if (request.IsPasswordGrantType())
            {
                var user = await _userManager.FindByNameAsync(request.Username ?? string.Empty);
                if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password ?? string.Empty))
                {
                    return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }

                var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, OpenIddictConstants.Claims.Name, OpenIddictConstants.Claims.Role);

                // Pridanie 'sub' claimu s platnou hodnotou a nastavenie destinácie
                identity.AddClaim(new Claim(OpenIddictConstants.Claims.Subject, user.Id.ToString())
                    .SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken));

                // Pridanie 'email' claimu a nastavenie destinácie
                identity.AddClaim(new Claim(OpenIddictConstants.Claims.Email, user.Email ?? string.Empty)
                    .SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken));

                identity.AddClaim(new Claim(OpenIddictConstants.Claims.Audience, "popelar-api")
                    .SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken));

                // Pridanie 'role' claimov a nastavenie destinácie
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role)
                        .SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken));
                }

                // Pridanie ďalších claimov a nastavenie destinácie
                identity.AddClaim(new Claim(OpenIddictConstants.Claims.Name, user.UserName ?? string.Empty)
                    .SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken));

                claimsPrincipal = new ClaimsPrincipal(identity);
                claimsPrincipal.SetScopes(request.GetScopes());
            }

            else
            if (request.IsClientCredentialsGrantType())
            {
                // Note: the client credentials are automatically validated by OpenIddict:
                // if client_id or client_secret are invalid, this action won't be invoked.

                var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                // Subject (sub) is a required field, we use the client id as the subject identifier here.
                identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId ?? throw new InvalidOperationException());

                // Add some claim, don't forget to add destination otherwise it won't be added to the access token.
                identity.AddClaim("some-claim", "some-value", OpenIddictConstants.Destinations.AccessToken);

                claimsPrincipal = new ClaimsPrincipal(identity);

                claimsPrincipal.SetScopes(request.GetScopes());
            }

            else if (request.IsAuthorizationCodeGrantType())
            {
                // Retrieve the claims principal stored in the authorization code
                claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme))?.Principal ?? new ClaimsPrincipal();
            }

            else if (request.IsRefreshTokenGrantType())
            {
                // Retrieve the claims principal stored in the refresh token.
                claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme))?.Principal ?? new ClaimsPrincipal();
            }

            else
            {
                throw new InvalidOperationException("The specified grant type is not supported.");
            }

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("~/connect/userinfo")]
        public async Task<IActionResult> Userinfo()
        {
            var claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

            return Ok(new
            {
                Name = claimsPrincipal?.GetClaim(OpenIddictConstants.Claims.Subject),
                Occupation = "Developer",
                Age = 43
            });
        }
    }
}
