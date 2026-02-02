
using ElTocardo.Authorization.Server.Endpoints;

namespace ElTocardo.Authorization.Server.Configuration;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureElTocardoAuthorizationServer(this WebApplication app)
    {

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            // GET {{MCP.WebApi_HostAddress}}/openapi/v1.json
            app.MapOpenApi();
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapRazorPages()
           .WithStaticAssets();
        app.MapConnectEndpoints();
        return app;
    }
}
