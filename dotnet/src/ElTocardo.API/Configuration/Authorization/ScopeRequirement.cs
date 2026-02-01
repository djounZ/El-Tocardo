using Microsoft.AspNetCore.Authorization;

namespace ElTocardo.API.Configuration.Authorization;

public class ScopeRequirement(params string[] scopes) : IAuthorizationRequirement
{
    public string[] Scopes { get; } = scopes;
}
