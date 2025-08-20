using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Mappers;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Queries;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using ElTocardo.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Handlers;

public class GetMcpServerByNameQueryHandler(
    IMcpServerConfigurationRepository repository,
    ILogger<GetMcpServerByNameQueryHandler> logger,
    McpServerConfigurationDomainDtoMapper mapper)
    : QueryHandlerBase<GetMcpServerByNameQuery, McpServerConfigurationItemDto>(logger)
{
    protected override async Task<McpServerConfigurationItemDto> HandleAsyncImplementation(
        GetMcpServerByNameQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting MCP server configuration: {ServerName}", query.ServerName);

        var configuration = await repository.GetByNameAsync(query.ServerName, cancellationToken);

        var result = mapper.MapDomainToDto(configuration!);

        logger.LogInformation("Retrieved MCP server configuration: {ServerName}", query.ServerName);
        return result;
    }
}
