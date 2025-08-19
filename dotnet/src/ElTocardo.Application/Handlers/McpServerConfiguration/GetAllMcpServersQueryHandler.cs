using ElTocardo.Application.Common.Extensions;
using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Queries.McpServerConfiguration;
using ElTocardo.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Handlers.McpServerConfiguration;

public class GetAllMcpServersQueryHandler(
    IMcpServerConfigurationRepository repository,
    ILogger<GetAllMcpServersQueryHandler> logger)
    : IQueryHandler<GetAllMcpServersQuery, IDictionary<string, McpServerConfigurationItemDto>>
{
    public async Task<IDictionary<string, McpServerConfigurationItemDto>> HandleAsync(
        GetAllMcpServersQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all MCP server configurations");

        var configurations = await repository.GetAllAsync(cancellationToken);

        var result = configurations.ToDictionary(
            config => config.ServerName,
            config => new McpServerConfigurationItemDto(
                config.Category,
                config.Command,
                config.Arguments,
                config.EnvironmentVariables,
                config.Endpoint,
                config.TransportType.ToDto()));

        logger.LogInformation("Retrieved {Count} MCP server configurations", result.Count);

        return result;
    }
}
