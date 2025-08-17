using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Services;
using ElTocardo.Infrastructure.Mappers.Dtos.ModelContextProtocol;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Client;

namespace ElTocardo.Infrastructure.Services;

public sealed class McpClientToolsService(ILogger<McpClientToolsService> logger, McpServerConfigurationDto mcpServerConfiguration, ClientTransportFactoryService clientTransportFactoryService, ModelContextProtocolMapper modelContextProtocolMapper ) : IMcpClientToolsService
{
    public async Task<IDictionary<string, IList<McpClientToolDto>>> GetAll(CancellationToken cancellationToken)
    {

        var mcpToolDescriptions = new Dictionary<string, IList<McpClientToolDto>>();
        var servers =  mcpServerConfiguration.Servers;

        foreach (var (serverName, serverConfiguration) in servers)
        {
            await using var client = await CreateMcpClientAsync(serverConfiguration, cancellationToken: cancellationToken);
            var mcpClientTools = await client.ListToolsAsync(cancellationToken: cancellationToken);

            var toolDescriptions = modelContextProtocolMapper.MapToMcpClientToolDtos(mcpClientTools);
            mcpToolDescriptions[serverName] = toolDescriptions;
        }

        return mcpToolDescriptions;
    }

    public async Task<CallToolResultDto> CallToolAsync(McpClientToolRequestDto request, CancellationToken cancellationToken)
    {
        McpServerConfigurationItemDto mcpServerConfigurationItem = mcpServerConfiguration.Servers[request.ServerName];
        await using var client = await CreateMcpClientAsync(mcpServerConfigurationItem, cancellationToken: cancellationToken);

        var callToolResult = await client.CallToolAsync(request.ToolName, request.Arguments, _progress, null, cancellationToken);
        return modelContextProtocolMapper.MapToCallToolResultDto(callToolResult);
    }

    private async Task<IMcpClient> CreateMcpClientAsync( McpServerConfigurationItemDto mcpServerConfigurationItem,
        CancellationToken cancellationToken )
    {
        var clientTransport = clientTransportFactoryService.Create(mcpServerConfigurationItem);
        return  await McpClientFactory.CreateAsync(clientTransport, cancellationToken: cancellationToken);
    }

    private readonly McpClientToolCallProgressIProgress _progress = new(logger);
    private sealed class McpClientToolCallProgressIProgress(ILogger logger) :  IProgress<ProgressNotificationValue>{
        public void Report(ProgressNotificationValue value)
        {
            logger.LogInformation("Progression Report {@ProgressNotificationValue}",value);
        }
    }
}
