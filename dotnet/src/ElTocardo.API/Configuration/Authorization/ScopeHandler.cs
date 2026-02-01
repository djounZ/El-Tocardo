using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;

namespace ElTocardo.API.Configuration.Authorization;

public class ScopeHandler : AuthorizationHandler<ScopeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, ScopeRequirement requirement)
    {
        // Check both "scope" (space-separated) and "scp" (OpenIddict array)
        var scopeClaim = context.User.FindFirst(OpenIddictConstants.Claims.Scope)?.Value;

        if (scopeClaim != null)
        {
            var grantedScopes = scopeClaim.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (requirement.Scopes.Any(s => grantedScopes.Contains(s)))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
