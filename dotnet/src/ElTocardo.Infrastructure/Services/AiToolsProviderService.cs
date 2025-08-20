using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Queries.McpServerConfiguration;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace ElTocardo.Infrastructure.Services;

public sealed class AiToolsProviderService(
    IQueryHandler<GetMcpServerByNameQuery, McpServerConfigurationItemDto> getByNameQueryHandler,
    ClientTransportFactoryService clientTransportFactoryService)
{
    public async Task<IList<AITool>> GetAiToolsAsync(IDictionary<string, IList<AiToolDto>> aiToolDtosByServer,
        CancellationToken cancellationToken)
    {
        var aiTools = new List<AITool>();
        foreach (var (server, aiToolDtos) in aiToolDtosByServer)
        {
            aiTools.AddRange(await GetAiToolsAsync(server, aiToolDtos, cancellationToken));
        }

        return aiTools;
    }

    private async Task<IEnumerable<AITool>> GetAiToolsAsync(string server, IList<AiToolDto> aiToolDtos,
        CancellationToken cancellationToken)
    {
        var mcpServerConfigurationItemDto =
            await getByNameQueryHandler.HandleAsync(new GetMcpServerByNameQuery(server), cancellationToken);

        if (!mcpServerConfigurationItemDto.IsSuccess)
        {
            throw mcpServerConfigurationItemDto.ReadError();
        }

        var clientTransport = clientTransportFactoryService.Create(mcpServerConfigurationItemDto.ReadValue());
        var client = await McpClientFactory.CreateAsync(clientTransport, cancellationToken: cancellationToken);
        var mcpClientTools = await client.ListToolsAsync(cancellationToken: cancellationToken);
        return mcpClientTools.Where(tool => aiToolDtos.Any(aiToolDto => aiToolDto.Name == tool.Name));
    }
}
