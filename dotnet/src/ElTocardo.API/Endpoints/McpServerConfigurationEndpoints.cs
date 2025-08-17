using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElTocardo.API.Endpoints;

public static class McpServerConfigurationEndpoints
{
    private static string Tags => "McpServerConfiguration";
    public static WebApplication MapMcpServerConfigurationEndpoints(this WebApplication app)
    {
        app.MapGet("v1/mcp-servers",
                ([FromServices] IMcpServerConfigurationProviderService service) => Results.Ok(service.GetAllServers()))
            .WithName("GetAllMcpServers")
            .WithSummary("Get all MCP servers")
            .WithDescription("Returns all MCP server configuration items")
            .WithTags(Tags)
            .Produces<IDictionary<string, McpServerConfigurationItemDto>>()
            .WithOpenApi();

        app.MapGet("v1/mcp-servers/{serverName}",
                ([FromServices] IMcpServerConfigurationProviderService service, string serverName) =>
                {
                    var item = service.GetServer(serverName);
                    return item is not null ? Results.Ok(item) : Results.NotFound();
                })
            .WithName("GetMcpServerByName")
            .WithSummary("Get MCP server by name")
            .WithDescription("Returns a single MCP server configuration item by name")
            .WithTags(Tags)
            .Produces<McpServerConfigurationItemDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        app.MapPost("v1/mcp-servers/{serverName}", ([FromServices] IMcpServerConfigurationProviderService service,
                string serverName, [FromBody] McpServerConfigurationItemDto item) =>
            {
                var created = service.CreateServer(serverName, item);
                return created
                    ? Results.Created($"/mcp-servers/{serverName}", item)
                    : Results.Conflict($"Server '{serverName}' already exists.");
            })
            .WithName("CreateMcpServer")
            .WithSummary("Create MCP server")
            .WithDescription("Creates a new MCP server configuration item")
            .WithTags(Tags)
            .Produces<McpServerConfigurationItemDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict)
            .WithOpenApi();

        app.MapPut("v1/mcp-servers/{serverName}", ([FromServices] IMcpServerConfigurationProviderService service,
                string serverName, [FromBody] McpServerConfigurationItemDto item) =>
            {
                var updated = service.UpdateServer(serverName, item);
                return updated ? Results.Ok(item) : Results.NotFound($"Server '{serverName}' not found.");
            })
            .WithName("UpdateMcpServer")
            .WithSummary("Update MCP server")
            .WithDescription("Updates an existing MCP server configuration item")
            .WithTags(Tags)
            .Produces<McpServerConfigurationItemDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        app.MapDelete("v1/mcp-servers/{serverName}",
                ([FromServices] IMcpServerConfigurationProviderService service, string serverName) =>
                {
                    var deleted = service.DeleteServer(serverName);
                    return deleted ? Results.NoContent() : Results.NotFound($"Server '{serverName}' not found.");
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
