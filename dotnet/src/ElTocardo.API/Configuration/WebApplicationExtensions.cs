using ElTocardo.API.Endpoints;

namespace ElTocardo.API.Configuration;

public static class WebApplicationExtensions
{
    public static WebApplication MapElTocardoApiEndpoints(this WebApplication app)
    {
        app.MapWeatherEndpoints();
        return app;
    }
}
