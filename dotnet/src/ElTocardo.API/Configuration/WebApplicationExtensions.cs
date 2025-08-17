using ElTocardo.API.Endpoints;

namespace ElTocardo.API.Configuration;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> ConfigureElTocardoApiAsync(this WebApplication app, CancellationToken cancellationToken)
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
        app.MapEndpoints();
        await app.Services.UseElTocardoApiAsync(cancellationToken);
        return app;
    }

    private static WebApplication UseCaching(this WebApplication app)
    {
        app.UseSession();
        app.UseOutputCache();
        return app;
    }

    private static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapDevelopmentTestEndpoints();
        app.MapMcpServerConfigurationEndpoints();
        app.MapAiProviderEndpoints();
        return app;
    }
}
