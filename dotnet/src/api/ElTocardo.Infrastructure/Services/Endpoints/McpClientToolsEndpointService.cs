using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Mappers.Dtos.ModelContextProtocol;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Queries;
using ElTocardo.Application.Services;
using ElTocardo.Domain.Mediator.Common.Interfaces;
using ElTocardo.Domain.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Client;

namespace ElTocardo.Infrastructure.Services.Endpoints;

public sealed class McpClientToolsEndpointService(
    ILogger<McpClientToolsEndpointService> logger,
    IQueryHandler<GetAllMcpServersQuery, Dictionary<string, McpServerConfigurationItemDto>> getAllQueryHandler,
    IQueryHandler<GetMcpServerByNameQuery, McpServerConfigurationItemDto> getByNameQueryHandler,
    ClientTransportFactoryService clientTransportFactoryService,
    ModelContextProtocolMapper modelContextProtocolMapper) : IMcpClientToolsEndpointService
{
    private readonly McpClientToolCallProgressIProgress _progress = new(logger);

    public async Task<Result<IDictionary<string, IList<McpClientToolDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var servers = await getAllQueryHandler.HandleAsync(GetAllMcpServersQuery.Instance, cancellationToken);

        if (!servers.IsSuccess)
        {
            return servers.ReadError();
        }

        var mcpToolDescriptionsTask = new Dictionary<string, Task<Result<McpClientToolDto[]>>>();
        foreach (var (serverName, serverConfiguration) in servers.ReadValue())
        {
            mcpToolDescriptionsTask[serverName] = GetMcpClientToolDtosAsync(serverConfiguration, cancellationToken);
        }

        var mcpToolDescriptions = new Dictionary<string, IList<McpClientToolDto>>();
        foreach (var (serverName, toolDescriptionsTask) in mcpToolDescriptionsTask)
        {
            var toolDescriptions = await toolDescriptionsTask;
            if(toolDescriptions.IsSuccess)
            {
                mcpToolDescriptions[serverName] = toolDescriptions.ReadValue();
            }
        }

        return mcpToolDescriptions;
    }

    private async Task<Result<McpClientToolDto[]>> GetMcpClientToolDtosAsync(
        McpServerConfigurationItemDto serverConfiguration, CancellationToken cancellationToken)
    {
        try
        {
            await using var client = await CreateMcpClientAsync(serverConfiguration, cancellationToken);
            var mcpClientTools = await client.ListToolsAsync(cancellationToken: cancellationToken);

            var toolDescriptions = modelContextProtocolMapper.ToApplication(mcpClientTools);
            return toolDescriptions;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting mcp client tools for {McpServerConfigurationItemDto}", serverConfiguration);
            return ex;
        }
    }

    public async Task<Result<CallToolResultDto>> CallToolAsync(McpClientToolRequestDto request,
        CancellationToken cancellationToken)
    {
        var mcpServerConfigurationItem =
            await getByNameQueryHandler.HandleAsync(new GetMcpServerByNameQuery(request.ServerName), cancellationToken);
        if (!mcpServerConfigurationItem.IsSuccess)
        {
            return mcpServerConfigurationItem.ReadError();
        }

        try
        {
            return await CallToolAsync(request, mcpServerConfigurationItem, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while calling tool: {McpClientToolRequestDto}", request);
            return ex;
        }
    }

    private async Task<CallToolResultDto> CallToolAsync(McpClientToolRequestDto request,
        Result<McpServerConfigurationItemDto> mcpServerConfigurationItem, CancellationToken cancellationToken)
    {
        await using var client = await CreateMcpClientAsync(mcpServerConfigurationItem.ReadValue(), cancellationToken);

        var callToolResult =
            await client.CallToolAsync(request.ToolName, request.Arguments, _progress, null, cancellationToken);
        return modelContextProtocolMapper.ToApplication(callToolResult);
    }

    private async Task<McpClient> CreateMcpClientAsync(McpServerConfigurationItemDto mcpServerConfigurationItem,
        CancellationToken cancellationToken)
    {
        var clientTransport = clientTransportFactoryService.Create(mcpServerConfigurationItem);
        return await McpClient.CreateAsync(clientTransport, cancellationToken: cancellationToken);
    }

    private sealed class McpClientToolCallProgressIProgress(ILogger logger) : IProgress<ProgressNotificationValue>
    {
        public void Report(ProgressNotificationValue value)
        {
            logger.LogInformation("Progression Report {@ProgressNotificationValue}", value);
        }
    }
}
