using AI.GithubCopilot.Domain.Services;
using ElTocardo.API.Options;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace ElTocardo.API.Endpoints;

public static class DevelopmentTestsEndpoints
{
    public static IEndpointRouteBuilder MapDevelopmentTestEndpoints(this IEndpointRouteBuilder app)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };




        app.MapGet("/test",async (
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                var s = context.Session.GetString("Test");
                if (s == null)
                {
                    await Task.Delay(10000, cancellationToken);
                    context.Session.SetString("Test", "Hello World");
                    return "Session value set to 'Hello World'";
                }
                else
                {
                    return $"Session value: {s}";
                }
            })
            .RequireAuthorization()
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks
                operation.Summary = "Gets the current weather report.";
                operation.Description = "Returns a short description and emoji.";
                return Task.CompletedTask;
            });
        app.MapGet("/",() => WeatherForecasts(summaries))
            .WithName("GetWeatherForecast")
            .WithSummary("Get weather forecast")
            .WithDescription("Returns a 5-day weather forecast")
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks
                operation.Summary = "Gets the current weather report.";
                operation.Description = "Returns a short description and emoji.";
                return Task.CompletedTask;
            })
            .CacheOutput(PredefinedOutputCachingPolicy.GlobalShortLiving);

        app.MapGet("/configuration",(IConfiguration configuration) => configuration.AsDictionary())
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks
                operation.Summary = "Gets the current weather report.";
                operation.Description = "Returns a short description and emoji.";
                return Task.CompletedTask;
            });


        app.MapGet("/github",async (
                GithubCopilotChatClient completionsService,
                CancellationToken cancellationToken) => await completionsService.GetResponseAsync(
                new ChatMessage(ChatRole.User, "Hello, how are you?"), null, cancellationToken))
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks
                operation.Summary = "Gets the current weather report.";
                operation.Description = "Returns a short description and emoji.";
                return Task.CompletedTask;
            });


        app.MapGet("/ollama",async (
                OllamaApiClient completionsService,
                CancellationToken cancellationToken) => await completionsService.GetResponseAsync(
                new ChatMessage(ChatRole.User, "Hello, how are you?")
            ,null, cancellationToken))
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks
                operation.Summary = "Gets the current weather report.";
                operation.Description = "Returns a short description and emoji.";
                return Task.CompletedTask;
            });
        return app;
    }
    extension(IConfiguration config)
    {
        public string ToJson()
        {
            var dict = config.AsDictionary();
            // Using Newtonsoft.Json
            return System.Text.Json.JsonSerializer.Serialize(dict, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        }

        private IDictionary<string, object?> AsDictionary()
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
