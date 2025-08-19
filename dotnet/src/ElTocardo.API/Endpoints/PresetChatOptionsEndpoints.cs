using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElTocardo.API.Endpoints;

public static class PresetChatOptionsEndpoints
{
    private static string Tags => "PresetChatOptions";
    public static WebApplication MapPresetChatOptionsEndpoints(this WebApplication app)
    {
        app.MapGet("v1/preset-chat-options",
                async ([FromServices] IPresetChatOptionsService service, CancellationToken cancellationToken) =>
                    Results.Ok(await service.GetAllAsync(cancellationToken)))
            .WithName("GetAllPresetChatOptions")
            .WithSummary("Get all preset chat options")
            .WithDescription("Returns all preset chat options items")
            .WithTags(Tags)
            .Produces<IEnumerable<PresetChatOptionsDto>>()
            .WithOpenApi();

        app.MapGet("v1/preset-chat-options/{name}",
                async ([FromServices] IPresetChatOptionsService service, string name, CancellationToken cancellationToken) =>
                {
                    var item = await service.GetByNameAsync(name, cancellationToken);
                    return item is not null ? Results.Ok(item) : Results.NotFound();
                })
            .WithName("GetPresetChatOptionsByName")
            .WithSummary("Get preset chat options by name")
            .WithDescription("Returns a single preset chat options item by name")
            .WithTags(Tags)
            .Produces<PresetChatOptionsDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        app.MapPost("v1/preset-chat-options/{name}", async ([FromServices] IPresetChatOptionsService service,
                string name, [FromBody] PresetChatOptionsDto item, CancellationToken cancellationToken) =>
            {
                var id = await service.CreateAsync(item, cancellationToken);
                // For simplicity, always return Created. You can add conflict logic if needed.
                return Results.Created($"/v1/preset-chat-options/{name}", item);
            })
            .WithName("CreatePresetChatOptions")
            .WithSummary("Create preset chat options")
            .WithDescription("Creates a new preset chat options item")
            .WithTags(Tags)
            .Produces<PresetChatOptionsDto>(StatusCodes.Status201Created)
            .WithOpenApi();

        app.MapPut("v1/preset-chat-options/{name}", async ([FromServices] IPresetChatOptionsService service,
                string name, [FromBody] PresetChatOptionsDto item, CancellationToken cancellationToken) =>
            {
                var result = await service.UpdateAsync(name, item, cancellationToken);
                return result
                    ? Results.Ok(item)
                    : Results.NotFound();
            })
            .WithName("UpdatePresetChatOptions")
            .WithSummary("Update preset chat options")
            .WithDescription("Updates an existing preset chat options item")
            .WithTags(Tags)
            .Produces<PresetChatOptionsDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        app.MapDelete("v1/preset-chat-options/{name}",
                async ([FromServices] IPresetChatOptionsService service, string name, CancellationToken cancellationToken) =>
                {
                    var result = await service.DeleteAsync(name, cancellationToken);
                    return result ? Results.NoContent() : Results.NotFound();
                })
            .WithName("DeletePresetChatOptions")
            .WithSummary("Delete preset chat options")
            .WithDescription("Deletes a preset chat options item")
            .WithTags(Tags)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        return app;
    }
}
