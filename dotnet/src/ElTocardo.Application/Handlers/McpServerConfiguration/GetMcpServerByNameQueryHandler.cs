using ElTocardo.Application.Common.Extensions;
using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Queries.McpServerConfiguration;
using ElTocardo.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Handlers.McpServerConfiguration;

public class GetMcpServerByNameQueryHandler(
    IMcpServerConfigurationRepository repository,
    ILogger<GetMcpServerByNameQueryHandler> logger)
    : IQueryHandler<GetMcpServerByNameQuery, McpServerConfigurationItemDto?>
{
    public async Task<McpServerConfigurationItemDto?> HandleAsync(
        GetMcpServerByNameQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting MCP server configuration: {ServerName}", query.ServerName);

        var configuration = await repository.GetByNameAsync(query.ServerName, cancellationToken);

        if (configuration == null)
        {
            logger.LogInformation("MCP server configuration not found: {ServerName}", query.ServerName);
            return null;
        }

        var result = new McpServerConfigurationItemDto(
            configuration.Category,
            configuration.Command,
            configuration.Arguments,
            configuration.EnvironmentVariables,
            configuration.Endpoint,
            configuration.TransportType.ToDto());

        logger.LogInformation("Retrieved MCP server configuration: {ServerName}", query.ServerName);

        return result;
    }
}
