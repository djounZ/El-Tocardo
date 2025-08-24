using ElTocardo.API.Options;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElTocardo.API.Endpoints;

public static class McpServerConfigurationEndpoints
{
    private static string Tags => "McpServerConfiguration";

    public static IEndpointRouteBuilder MapMcpServerConfigurationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("v1/mcp-servers",
                async ([FromServices] IMcpServerConfigurationEndpointService service, CancellationToken cancellationToken) =>
                {
                    var allServersAsync = await service.GetAllServersAsync(cancellationToken);
                    return allServersAsync.IsSuccess
                        ? Results.Ok(allServersAsync.ReadValue())
                        : Results.InternalServerError(allServersAsync.ReadError());
                })
            .WithName("GetAllMcpServers")
            .WithSummary("Get all MCP servers")
            .WithDescription("Returns all MCP server configuration items")
            .WithTags(Tags)
            .Produces<IDictionary<string, McpServerConfigurationItemDto>>()
            .WithOpenApi()
            .CacheOutput(PredefinedOutputCachingPolicy.PerUserVaryByHeaderAuthorizationShortLiving);

        app.MapGet("v1/mcp-servers/{serverName}",
                async ([FromServices] IMcpServerConfigurationEndpointService service, string serverName,
                    CancellationToken cancellationToken) =>
                {
                    var item = await service.GetServerAsync(serverName, cancellationToken);
                    return item.IsSuccess ? Results.Ok(item.ReadValue()) : Results.NotFound(item.ReadError());
                })
            .WithName("GetMcpServerByName")
            .WithSummary("Get MCP server by name")
            .WithDescription("Returns a single MCP server configuration item by name")
            .WithTags(Tags)
            .Produces<McpServerConfigurationItemDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        app.MapPost("v1/mcp-servers/{serverName}", async ([FromServices] IMcpServerConfigurationEndpointService service,
                string serverName, [FromBody] McpServerConfigurationItemDto item,
                CancellationToken cancellationToken) =>
            {
                var result = await service.CreateServerAsync(serverName, item, cancellationToken);
                return result.IsSuccess
                    ? Results.Created($"/v1/mcp-servers/{serverName}", item)
                    : Results.Conflict(result.ReadError().Message);
            })
            .WithName("CreateMcpServer")
            .WithSummary("Create MCP server")
            .WithDescription("Creates a new MCP server configuration item")
            .WithTags(Tags)
            .Produces<McpServerConfigurationItemDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict)
            .WithOpenApi();

        app.MapPut("v1/mcp-servers/{serverName}", async ([FromServices] IMcpServerConfigurationEndpointService service,
                string serverName, [FromBody] McpServerConfigurationItemDto item,
                CancellationToken cancellationToken) =>
            {
                var result = await service.UpdateServerAsync(serverName, item, cancellationToken);
                return result.IsSuccess
                    ? Results.Ok(item)
                    : Results.NotFound(result.ReadError());
            })
            .WithName("UpdateMcpServer")
            .WithSummary("Update MCP server")
            .WithDescription("Updates an existing MCP server configuration item")
            .WithTags(Tags)
            .Produces<McpServerConfigurationItemDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        app.MapDelete("v1/mcp-servers/{serverName}",
                async ([FromServices] IMcpServerConfigurationEndpointService service, string serverName,
                    CancellationToken cancellationToken) =>
                {
                    var result = await service.DeleteServerAsync(serverName, cancellationToken);
                    return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.ReadError());
                })
            .WithName("DeleteMcpServer")
            .WithSummary("Delete MCP server")
            .WithDescription("Deletes an MCP server configuration item")
            .WithTags(Tags)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        return app;
    }
}
