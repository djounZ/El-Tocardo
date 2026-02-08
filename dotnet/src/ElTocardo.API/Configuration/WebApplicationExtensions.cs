using ElTocardo.API.Endpoints;
using Scalar.AspNetCore;

using static ElTocardo.ServiceDefaults.Constants;
namespace ElTocardo.API.Configuration;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public async Task<WebApplication> ConfigureElTocardoApiAsync(CancellationToken cancellationToken)
        {

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                // GET {{MCP.WebApi_HostAddress}}/openapi/v1.json
                app.MapOpenApi();
                app.MapScalarApiReference("/scalar", options =>
                {
                    options
                        .AddPreferredSecuritySchemes(OpenIddictElTocardoApiSchemaKey) // This is the schemaKey from above
                        .AddClientCredentialsFlow(
                            OpenIddictElTocardoApiSchemaKey, // Again: schemaKey
                            flow =>
                            {
                                flow.ClientId = "postman";
                                flow.SelectedScopes =
                                    [OpenIddictElTocardoApiUserScope]; // Same scopes as defined in the OpenApi transformer!
                            }
                        ).AddAuthorizationCodeFlow(OpenIddictElTocardoApiSchemaKey, flow =>
                        {
                            flow.ClientId = "El-Tocardo-Assistant";
                            flow.SelectedScopes =
                                [OpenIddictElTocardoApiUserScope];
                            flow.RedirectUri = "http://localhost:4200/auth-callback";
                            flow.Pkce = Pkce.Sha256;
                        });
                });
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();
            app.UseCors("DefaultCorsPolicy");
            app.UseCaching();
            await app.Services.UseElTocardoApiAsync(cancellationToken);
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapEndpoints();
            return app;
        }

        private WebApplication UseCaching()
        {
            app.UseSession();
            app.UseOutputCache();
            return app;
        }

        private void MapEndpoints()
        {
            var routes = app.MapGroup(string.Empty).RequireAuthorization();
            routes
                .MapAiProviderEndpoints();

            var routesExluded = app.MapGroup(string.Empty).RequireAuthorization().ExcludeFromDescription();
            routesExluded
                .MapDevelopmentTestEndpoints()
                .MapMcpServerConfigurationEndpoints()
                .MapMcpClientToolsEndpoints()
                .MapChatCompletionsEndpoints()
                .MapConversationEndpoints()
                .MapPresetChatInstructionEndpoints()
                .MapPresetChatOptionsEndpoints();
        }
    }
}
