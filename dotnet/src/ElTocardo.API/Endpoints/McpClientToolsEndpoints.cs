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
                return response.IsSuccess ? Results.Ok(response.ReadValue()) :  Results.InternalServerError(response.ReadError());
            })
            .WithName("GetMcpToolDescriptions")
            .WithSummary("Get description of MCP tools")
            .WithDescription("Return all available MCP tools descriptions")
            .WithTags(Tags)
            .Produces<IDictionary<string, IList<McpClientToolDto>>>()
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks
                operation.Summary = "Gets the current weather report.";
                operation.Description = "Returns a short description and emoji.";
                return Task.CompletedTask;
            })
            .CacheOutput(PredefinedOutputCachingPolicy.PerUserVaryByHeaderAuthorizationLongLiving);

        app.MapPost("/api/mcp_tools/call", async (
                IMcpClientToolsEndpointService mcpClientToolProviderService,
                McpClientToolRequestDto request,
                CancellationToken cancellationToken) =>
            {
                var response = await mcpClientToolProviderService.CallToolAsync(request, cancellationToken);
                return response.IsSuccess ? Results.Ok(response.ReadValue()) :  Results.InternalServerError(response.ReadError());
            })
            .WithName("CallMcpTool")
            .WithSummary("Call an MCP tool")
            .WithDescription("Invoke a tool on a configured MCP server and return the result.")
            .WithTags(Tags)
            .Accepts<McpClientToolRequestDto>("application/json")
            .Produces<CallToolResultDto>()
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                // Per-endpoint tweaks
                operation.Summary = "Gets the current weather report.";
                operation.Description = "Returns a short description and emoji.";
                return Task.CompletedTask;
            });

        return app;
    }
}
