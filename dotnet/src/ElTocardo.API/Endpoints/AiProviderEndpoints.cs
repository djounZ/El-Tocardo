using System.Reflection;
using System.Text.Json.Serialization;
using ElTocardo.Application.Dtos.Provider;
using ElTocardo.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElTocardo.API.Endpoints;

public static class AiProviderEndpoints
{
    private static string Tags => "AiProvider";
    public static WebApplication MapAiProviderEndpoints(this WebApplication app)
    {
        app.MapGet("v1/ai-providers",
                async ([FromServices] IAiProviderService service, CancellationToken cancellationToken) => Results.Ok(await service.GetAllAsync(cancellationToken)))
            .WithName("GetAllAiProvider")
            .WithSummary("Get all AI Provider")
            .WithDescription("Returns all AI Provider items")
            .WithTags(Tags)
            .Produces<AiProviderDto[]>()
            .WithOpenApi();

        app.MapGet("v1/ai-providers/{provider}",
                async ([FromServices] IAiProviderService service, string provider, CancellationToken cancellationToken) =>
                {
                    var item = await service.GetAsync(provider.Parse<AiProviderEnumDto>(), cancellationToken);
                    return item is not null ? Results.Ok(item) : Results.NotFound();
                })
            .WithName("GetAiProvider")
            .WithSummary("Get AI Provider")
            .WithDescription("Returns a single AI Provider item")
            .WithTags(Tags)
            .Produces<AiProviderDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        return app;
    }


    public static TEnum Parse<TEnum>(this string value) where TEnum : struct, Enum
    {
        foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attr = field.GetCustomAttribute<JsonStringEnumMemberNameAttribute>();
            if (attr != null && string.Equals(attr.Name, value, StringComparison.OrdinalIgnoreCase))
            {
                return (TEnum)field.GetValue(null)!;
            }
        }
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid string value for enum conversion.");
    }
}
