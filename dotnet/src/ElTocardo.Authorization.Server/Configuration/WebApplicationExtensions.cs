using ElTocardo.Authorization.Server.Endpoints;

namespace ElTocardo.Authorization.Server.Configuration;

public static class WebApplicationExtensions
{
    public static  WebApplication ConfigureElTocardoAuthorizationServer(this WebApplication app)
    {

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            // GET {{MCP.WebApi_HostAddress}}/openapi/v1.json
            app.MapOpenApi();
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.MapEndpoints();
        return app;
    }

    private static void MapEndpoints(this WebApplication app)
    {
        app.MapAuthorizationEndpoints()
            ;
    }
}
