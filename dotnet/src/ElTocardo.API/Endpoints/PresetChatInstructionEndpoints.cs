using ElTocardo.Application.Services;
using ElTocardo.Application.Dtos.PresetChatInstruction;
using Microsoft.AspNetCore.Mvc;

namespace ElTocardo.API.Endpoints;

public static class PresetChatInstructionEndpoints
{
    private static string Tags => "Preset Chat Instructions";

    public static IEndpointRouteBuilder MapPresetChatInstructionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("v1/preset-chat-instructions",
            async ([FromServices] IPresetChatInstructionEndpointService service, CancellationToken cancellationToken) =>
            {
                var result = await service.GetAllAsync(cancellationToken);
                return result.IsSuccess
                    ? Results.Ok(result.ReadValue())
                    : Results.InternalServerError(result.ReadError());
            })
            .WithName("GetAllPresetChatInstructions")
            .WithSummary("Get all preset chat instructions")
            .WithDescription("Returns all preset chat instructions")
            .WithTags(Tags)
            .Produces<IList<PresetChatInstructionDto>>()
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });

        app.MapGet("v1/preset-chat-instructions/{name}",
            async ([FromServices] IPresetChatInstructionEndpointService service, string name, CancellationToken cancellationToken) =>
            {
                var result = await service.GetByNameAsync(name, cancellationToken);
                return result.IsSuccess
                    ? Results.Ok(result.ReadValue())
                    : Results.NotFound(result.ReadError());
            })
            .WithName("GetPresetChatInstructionByName")
            .WithSummary("Get preset chat instruction by name")
            .WithDescription("Returns a single preset chat instruction by name")
            .WithTags(Tags)
            .Produces<PresetChatInstructionDto>()
            .Produces(StatusCodes.Status404NotFound)
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });

        app.MapPost("v1/preset-chat-instructions/{name}",
            async ([FromServices] IPresetChatInstructionEndpointService service, string name, [FromBody] PresetChatInstructionDto dto, CancellationToken cancellationToken) =>
            {
                var result = await service.CreateAsync(dto.Name, dto.Description, dto.ContentType, dto.Content, cancellationToken);
                return result.IsSuccess
                    ? Results.Created($"/v1/preset-chat-instructions/{name}", dto)
                    : Results.Conflict(result.ReadError().Message);
            })
            .WithName("CreatePresetChatInstruction")
            .WithSummary("Create preset chat instruction")
            .WithDescription("Creates a new preset chat instruction")
            .WithTags(Tags)
            .Produces<PresetChatInstructionDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict)
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });

        app.MapPut("v1/preset-chat-instructions/{name}",
            async ([FromServices] IPresetChatInstructionEndpointService service, string name, [FromBody] PresetChatInstructionDto dto, CancellationToken cancellationToken) =>
            {
                var result = await service.UpdateAsync(name, dto.Description, dto.ContentType, dto.Content, cancellationToken);
                return result.IsSuccess
                    ? Results.Ok(dto)
                    : Results.NotFound(result.ReadError());
            })
            .WithName("UpdatePresetChatInstruction")
            .WithSummary("Update preset chat instruction")
            .WithDescription("Updates an existing preset chat instruction")
            .WithTags(Tags)
            .Produces<PresetChatInstructionDto>()
            .Produces(StatusCodes.Status404NotFound)
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks


                return Task.CompletedTask;
            });

        app.MapDelete("v1/preset-chat-instructions/{name}",
            async ([FromServices] IPresetChatInstructionEndpointService service, string name, CancellationToken cancellationToken) =>
            {
                var result = await service.DeleteAsync(name, cancellationToken);
                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.NotFound(result.ReadError());
            })
            .WithName("DeletePresetChatInstruction")
            .WithSummary("Delete preset chat instruction")
            .WithDescription("Deletes a preset chat instruction by name")
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
