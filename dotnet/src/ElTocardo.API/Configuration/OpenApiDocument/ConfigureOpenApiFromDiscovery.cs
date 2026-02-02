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
        var scopes = new Dictionary<string, string>
        {
            {
                OpenIddictElTocardoApiUserScope, // Actual Scope
                "All scopes" // Human readable description
            },
        };
        var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            [schemaKey] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Scheme = schemaKey,
                Flows = new OpenApiOAuthFlows
                {
                    ClientCredentials = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(jsonElement.GetProperty("authorization_endpoint").GetString()!),
                        TokenUrl = new Uri(jsonElement.GetProperty("token_endpoint").GetString()!),
                        RefreshUrl = new Uri(jsonElement.GetProperty("token_endpoint").GetString()!),
                        Scopes = scopes,
                    },
                },
            },
        };
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = securitySchemes;
        var securityRequirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(schemaKey, document)] = [..scopes.Keys],
        };

        document.Security = [securityRequirement];
    }
}
