using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;

namespace ElTocardo.Authorization.Server.Handlers;

public sealed class CustomTokenRequestHandler :
    IOpenIddictServerHandler<OpenIddictServerEvents.HandleTokenRequestContext>
{
    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.HandleTokenRequestContext>()
            .UseScopedHandler<CustomTokenRequestHandler>()
            .SetOrder(1000) // After built-in validation
            .SetType(OpenIddictServerHandlerType.BuiltIn)
            .Build();

    public async ValueTask HandleAsync(OpenIddictServerEvents.HandleTokenRequestContext context)
    {
        var request = context.Request;
        ClaimsPrincipal claimsPrincipal;
        if (request.IsAuthorizationCodeGrantType() )
        {  var httpContext = context.Transaction.GetHttpRequest()?.HttpContext
                             ?? throw new InvalidOperationException("HttpContext not available");
            claimsPrincipal = (await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal ?? new ClaimsPrincipal();
        }
        else if (request.IsClientCredentialsGrantType() || request.IsRefreshTokenGrantType())
        {
            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId ?? throw new InvalidOperationException());
            // Optionally add custom claims for client credentials flow
            claimsPrincipal = new ClaimsPrincipal(identity);
            claimsPrincipal.SetScopes(request.GetScopes());
        }
        else
        {
            context.Reject(error: OpenIddictConstants.Errors.UnsupportedGrantType,
                description: $"The specified grant type is not implemented. {request.GrantType}");
            return;
        }
        context.SignIn(claimsPrincipal);
    }
}
