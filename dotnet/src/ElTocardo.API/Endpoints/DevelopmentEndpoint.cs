using AI.GithubCopilot.Domain.Services;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Services;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace ElTocardo.API.Endpoints;

public static class DevelopmentEndpoint
{
    /// <summary>
    ///     Maps weather forecast endpoints to the application
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application for method chaining</returns>
    public static WebApplication MapWeatherEndpoints(this WebApplication app)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/",() => WeatherForecasts(summaries))
            .WithName("GetWeatherForecast")
            .WithSummary("Get weather forecast")
            .WithDescription("Returns a 5-day weather forecast")
            .WithOpenApi();

        app.MapGet("/configuration",(IConfiguration configuration) => configuration.AsDictionary())
            .WithOpenApi();


        app.MapGet("/github",async (
                GithubCopilotChatClient completionsService,
                CancellationToken cancellationToken) => await completionsService.GetResponseAsync(
                new ChatMessage(ChatRole.User, "Hello, how are you?"), null, cancellationToken))
            .WithOpenApi();


        app.MapGet("/ollama",async (
                OllamaApiClient completionsService,
                CancellationToken cancellationToken) => await completionsService.GetResponseAsync(
                new ChatMessage(ChatRole.User, "Hello, how are you?")
            ,null, cancellationToken))
            .WithOpenApi();
        return app;
    }
    public static string ToJson(this IConfiguration config)
    {
        var dict = config.AsDictionary();
        // Using Newtonsoft.Json
         return System.Text.Json.JsonSerializer.Serialize(dict, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    }
    public static IDictionary<string, object?> AsDictionary(this IConfiguration config)
    {
        var result = new Dictionary<string, object?>();
        foreach (var child in config.GetChildren())
        {
            if (child.GetChildren().Any())
            {
                result[child.Key] = AsDictionary(child);
            }
            else
            {
                result[child.Key] = child.Value;
            }
        }
        return result;
    }
    private static WeatherForecast[] WeatherForecasts(string[] summaries)
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    }
}

/// <summary>
///     Weather forecast record
/// </summary>
/// <param name="Date">The date of the forecast</param>
/// <param name="TemperatureC">Temperature in Celsius</param>
/// <param name="Summary">Weather summary</param>
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    /// <summary>
    ///     Temperature in Fahrenheit
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
