using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Queries.McpServerConfiguration;
using ElTocardo.Application.Services;
using ElTocardo.Infrastructure.Mappers.Dtos.ModelContextProtocol;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Client;

namespace ElTocardo.Infrastructure.Services;

public sealed class McpClientToolsService(
    ILogger<McpClientToolsService> logger,
    IQueryHandler<GetAllMcpServersQuery, IDictionary<string, McpServerConfigurationItemDto>> getAllQueryHandler,
    IQueryHandler<GetMcpServerByNameQuery, McpServerConfigurationItemDto> getByNameQueryHandler,
    ClientTransportFactoryService clientTransportFactoryService,
    ModelContextProtocolMapper modelContextProtocolMapper) : IMcpClientToolsService
{
    private readonly McpClientToolCallProgressIProgress _progress = new(logger);

    public async Task<IDictionary<string, IList<McpClientToolDto>>> GetAll(CancellationToken cancellationToken)
    {
        var mcpToolDescriptions = new Dictionary<string, IList<McpClientToolDto>>();
        var servers = await getAllQueryHandler.HandleAsync(GetAllMcpServersQuery.Instance, cancellationToken);

        if (!servers.IsSuccess)
        {
            throw servers.ReadError();
        }

        foreach (var (serverName, serverConfiguration) in servers.ReadValue())
        {
            await using var client = await CreateMcpClientAsync(serverConfiguration, cancellationToken);
            var mcpClientTools = await client.ListToolsAsync(cancellationToken: cancellationToken);

            var toolDescriptions = modelContextProtocolMapper.MapToMcpClientToolDtos(mcpClientTools);
            mcpToolDescriptions[serverName] = toolDescriptions;
        }

        return mcpToolDescriptions;
    }

    public async Task<CallToolResultDto> CallToolAsync(McpClientToolRequestDto request,
        CancellationToken cancellationToken)
    {
        var mcpServerConfigurationItem =
            await getByNameQueryHandler.HandleAsync(new GetMcpServerByNameQuery(request.ServerName), cancellationToken);
        if (!mcpServerConfigurationItem.IsSuccess)
        {
            throw mcpServerConfigurationItem.ReadError();
        }

        await using var client = await CreateMcpClientAsync(mcpServerConfigurationItem.ReadValue(), cancellationToken);

        var callToolResult =
            await client.CallToolAsync(request.ToolName, request.Arguments, _progress, null, cancellationToken);
        return modelContextProtocolMapper.MapToCallToolResultDto(callToolResult);
    }

    private async Task<IMcpClient> CreateMcpClientAsync(McpServerConfigurationItemDto mcpServerConfigurationItem,
        CancellationToken cancellationToken)
    {
        var clientTransport = clientTransportFactoryService.Create(mcpServerConfigurationItem);
        return await McpClientFactory.CreateAsync(clientTransport, cancellationToken: cancellationToken);
    }

    private sealed class McpClientToolCallProgressIProgress(ILogger logger) : IProgress<ProgressNotificationValue>
    {
        public void Report(ProgressNotificationValue value)
        {
            logger.LogInformation("Progression Report {@ProgressNotificationValue}", value);
        }
    }
}
