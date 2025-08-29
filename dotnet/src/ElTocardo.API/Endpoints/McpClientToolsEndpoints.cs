using ElTocardo.API.Options;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Services;

namespace ElTocardo.API.Endpoints;

public static class McpClientToolsEndpoints
{

    private static string Tags => "Mcp Client Tools";

    public static IEndpointRouteBuilder MapMcpClientToolsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/mcp_tools", async (
                IMcpClientToolsEndpointService mcpClientToolProviderService,
                CancellationToken cancellationToken) =>
            {
                var response = await mcpClientToolProviderService.GetAll(cancellationToken);
                return Results.Ok(response);
            })
            .WithName("GetMcpToolDescriptions")
            .WithSummary("Get description of MCP tools")
            .WithDescription("Return all available MCP tools descriptions")
            .WithTags(Tags)
            .Produces<IDictionary<string, IList<McpClientToolDto>>>()
            .WithOpenApi()
            .CacheOutput(PredefinedOutputCachingPolicy.PerUserVaryByHeaderAuthorizationLongLiving);

        app.MapPost("/api/mcp_tools/call", async (
                IMcpClientToolsEndpointService mcpClientToolProviderService,
                McpClientToolRequestDto request,
                CancellationToken cancellationToken) =>
            {
                var response = await mcpClientToolProviderService.CallToolAsync(request, cancellationToken);
                return Results.Ok(response);
            })
            .WithName("CallMcpTool")
            .WithSummary("Call an MCP tool")
            .WithDescription("Invoke a tool on a configured MCP server and return the result.")
            .WithTags(Tags)
            .Accepts<McpClientToolRequestDto>("application/json")
            .Produces<CallToolResultDto>()
            .WithOpenApi();

        return app;
    }
}
