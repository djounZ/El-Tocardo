using ElTocardo.API.Endpoints;

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
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
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
                .MapDevelopmentTestEndpoints()
                .MapMcpServerConfigurationEndpoints()
                .MapAiProviderEndpoints()
                .MapMcpClientToolsEndpoints()
                .MapChatCompletionsEndpoints()
                .MapConversationEndpoints()
                .MapPresetChatInstructionEndpoints()
                .MapPresetChatOptionsEndpoints();
        }
    }
}
