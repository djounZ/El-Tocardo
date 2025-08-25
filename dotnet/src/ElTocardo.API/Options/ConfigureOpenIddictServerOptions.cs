using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;

namespace ElTocardo.API.Options;

public sealed class ConfigureOpenIddictServerOptions(IOptions<ElTocardoApiOptions> infrastructureOptions) :  IConfigureNamedOptions<OpenIddict.Server.OpenIddictServerOptions>
{
    private  OpenIddictServerOptions OpenIddictServerOptions => infrastructureOptions.Value.OpenIddictServerOptions;

    public void Configure(OpenIddict.Server.OpenIddictServerOptions options)
    {
        Configure(null, options);
    }

    public void Configure(string? name, OpenIddict.Server.OpenIddictServerOptions options)
    {
        var tokenEndpointUri = new Uri(OpenIddictServerOptions.TokenEndpointUri, UriKind.RelativeOrAbsolute);
        var authorizationEndpointUri = new Uri(OpenIddictServerOptions.AuthorizationEndpointUri, UriKind.RelativeOrAbsolute);
        var endSessionEndpointUri = new Uri(OpenIddictServerOptions.EndSessionEndpointUri, UriKind.RelativeOrAbsolute);
        var userInfoEndpointUri = new Uri(OpenIddictServerOptions.UserInfoEndpointUri, UriKind.RelativeOrAbsolute);
        var introspectionEndpointUri = new Uri(OpenIddictServerOptions.IntrospectionEndpointUri, UriKind.RelativeOrAbsolute);


        options.TokenEndpointUris.Add(tokenEndpointUri);
        options.AuthorizationEndpointUris.Add(authorizationEndpointUri);
        options.EndSessionEndpointUris.Add(endSessionEndpointUri);
        options.UserInfoEndpointUris.Add(userInfoEndpointUri);
        options.IntrospectionEndpointUris.Add(introspectionEndpointUri);
    }
}
