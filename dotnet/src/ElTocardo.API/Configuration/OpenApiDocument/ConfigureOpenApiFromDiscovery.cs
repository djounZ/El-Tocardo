using System.Text.Json;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.OpenApi;
using static ElTocardo.ServiceDefaults.Constants;

namespace ElTocardo.API.Configuration.OpenApiDocument;

public class DiscoverySecurityTransformer(HttpClient httpClient,IConfiguration config) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(Microsoft.OpenApi.OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    { // Fetch discovery document
        var resourceUrl = config[OpenIddictIssuerEnvironmentVariableName]!;

        var authServerUrl = new Uri($"{resourceUrl}/.well-known/openid-configuration");
        var jsonElement = await httpClient.GetFromJsonAsync<JsonElement>(authServerUrl, cancellationToken);


        var schemaKey = OpenIddictElTocardoApiSchemaKey;
        var oidc = "oidc";
        var scopes = new Dictionary<string, string>
        {
            {
                OpenIddictElTocardoApiUserScope, // Actual Scope
                "All scopes" // Human readable description
            },
        };

        var authorizationEndpoint = jsonElement.GetProperty("authorization_endpoint").GetString()!;
        var authorizationEndpointUri = new Uri(authorizationEndpoint);
        var tokenEndpoint = jsonElement.GetProperty("token_endpoint").GetString()!;
        var tokenEndpointUri = new Uri(tokenEndpoint);
        var openApiOAuthFlow = CreateOpenApiOAuthFlow(authorizationEndpointUri, tokenEndpointUri, scopes);
        var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            [oidc] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OpenIdConnect,
                OpenIdConnectUrl = authServerUrl,
                Description = "OpenID Connect authentication",
                Scheme = oidc,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = openApiOAuthFlow,
                    ClientCredentials = openApiOAuthFlow
                },
            },
            [schemaKey] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                OpenIdConnectUrl = authServerUrl,
                Description = "OAuth2 authentication",
                Scheme = schemaKey,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = openApiOAuthFlow,
                    ClientCredentials = openApiOAuthFlow
                },
            },
        };
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = securitySchemes;
        var securityRequirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(oidc, document)] = [..scopes.Keys],
            [new OpenApiSecuritySchemeReference(schemaKey, document)] = [..scopes.Keys],
        };

        document.Security = [securityRequirement];
    }

    private static OpenApiOAuthFlow CreateOpenApiOAuthFlow(Uri authorizationEndpointUri, Uri tokenEndpointUri, Dictionary<string, string> scopes)
    {
        return new OpenApiOAuthFlow
        {
            AuthorizationUrl = authorizationEndpointUri,
            TokenUrl = tokenEndpointUri,
            RefreshUrl = tokenEndpointUri,
            Scopes = scopes,
        };
    }
}
