using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElTocardo.API.Endpoints;

public static class PresetChatOptionsEndpoints
{
    private static string Tags => "Preset ChatOptions";
    public static IEndpointRouteBuilder MapPresetChatOptionsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("v1/preset-chat-options",
                async ([FromServices] IPresetChatOptionsEndpointService service, CancellationToken cancellationToken) =>
                {
                    var result = await service.GetAllAsync(cancellationToken);
                    return result.IsSuccess ? Results.Ok(result.ReadValue()) : Results.InternalServerError(result.ReadError());
                })
            .WithName("GetAllPresetChatOptions")
            .WithSummary("Get all preset chat options")
            .WithDescription("Returns all preset chat options items")
            .WithTags(Tags)
            .Produces<IEnumerable<PresetChatOptionsDto>>()
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });

        app.MapGet("v1/preset-chat-options/{name}",
                async ([FromServices] IPresetChatOptionsEndpointService service, string name, CancellationToken cancellationToken) =>
                {
                    var result = await service.GetByNameAsync(name, cancellationToken);
                    return result.IsSuccess ? Results.Ok(result.ReadValue()) : Results.NotFound(result.ReadError());
                })
            .WithName("GetPresetChatOptionsByName")
            .WithSummary("Get preset chat options by name")
            .WithDescription("Returns a single preset chat options item by name")
            .WithTags(Tags)
            .Produces<PresetChatOptionsDto>()
            .Produces(StatusCodes.Status404NotFound)
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });

        app.MapPost("v1/preset-chat-options/{name}", async ([FromServices] IPresetChatOptionsEndpointService service,
                string name, [FromBody] PresetChatOptionsDto item, CancellationToken cancellationToken) =>
            {
                var result = await service.CreateAsync(item, cancellationToken);
                return result.IsSuccess
                    ? Results.Created($"/v1/preset-chat-options/{name}", item)
                    : Results.Conflict(result.ReadError().Message);
            })
            .WithName("CreatePresetChatOptions")
            .WithSummary("Create preset chat options")
            .WithDescription("Creates a new preset chat options item")
            .WithTags(Tags)
            .Produces<PresetChatOptionsDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict)
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });

        app.MapPut("v1/preset-chat-options/{name}", async ([FromServices] IPresetChatOptionsEndpointService service,
                string name, [FromBody] PresetChatOptionsDto item, CancellationToken cancellationToken) =>
            {
                var result = await service.UpdateAsync(name, item, cancellationToken);
                return result.IsSuccess
                    ? Results.Ok(item)
                    : Results.NotFound(result.ReadError());
            })
            .WithName("UpdatePresetChatOptions")
            .WithSummary("Update preset chat options")
            .WithDescription("Updates an existing preset chat options item")
            .WithTags(Tags)
            .Produces<PresetChatOptionsDto>()
            .Produces(StatusCodes.Status404NotFound)
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });

        app.MapDelete("v1/preset-chat-options/{name}",
                async ([FromServices] IPresetChatOptionsEndpointService service, string name, CancellationToken cancellationToken) =>
                {
                    var result = await service.DeleteAsync(name, cancellationToken);
                    return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.ReadError());
                })
            .WithName("DeletePresetChatOptions")
            .WithSummary("Delete preset chat options")
            .WithDescription("Deletes a preset chat options item")
            .WithTags(Tags)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });

        return app;
    }
}
