using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ElTocardo.Authorization.Server.Handlers;

public class CustomEndSessionHandler : IOpenIddictServerHandler<OpenIddictServerEvents.HandleEndSessionRequestContext>
{
    private readonly IOpenIddictTokenManager _tokenManager;

    public CustomEndSessionHandler(IOpenIddictTokenManager tokenManager)
    {
        _tokenManager = tokenManager;
    }

    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.HandleEndSessionRequestContext>()
            .UseScopedHandler<CustomEndSessionHandler>()
            .SetOrder(1000_000)
            .SetType(OpenIddictServerHandlerType.Custom)
            .Build();

    public async ValueTask HandleAsync(OpenIddictServerEvents.HandleEndSessionRequestContext context)
    {
        var httpContext = context.Transaction.GetHttpRequest()?.HttpContext
                        ?? throw new InvalidOperationException("HttpContext not available");
        var postLogoutRedirectUri = context.Request.PostLogoutRedirectUri;

        if (string.IsNullOrEmpty(postLogoutRedirectUri))
        {
            context.Reject(Errors.InvalidRequest, "post_logout_redirect_uri required");
            return;
        }

        // 1. Get authenticated principal (user session)
        var result = await httpContext.AuthenticateAsync();
        if (!result.Succeeded)
        {
            // User not logged in → nothing to logout → 204 No Content + redirect
            context.Reject(Errors.InvalidRequest, "not authenticated");
            return;
        }

        var principal = result.Principal;
        var subject = principal.GetClaim(Claims.Subject)!;

        await _tokenManager.RevokeBySubjectAsync(subject);

        // 3. Sign out cookies
        await httpContext.SignOutAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        await httpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

    }
}
