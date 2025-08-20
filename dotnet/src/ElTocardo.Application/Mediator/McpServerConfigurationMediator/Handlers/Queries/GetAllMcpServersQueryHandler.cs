using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Mediator.Common.Handlers;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Mappers;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Queries;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Handlers.Queries;

public class GetAllMcpServersQueryHandler(
    IMcpServerConfigurationRepository repository,
    ILogger<GetAllMcpServersQueryHandler> logger,
    McpServerConfigurationDomainGetAllDtoMapper mapper)
    : QueryHandlerBase<GetAllMcpServersQuery, Dictionary<string, McpServerConfigurationItemDto>>(logger)
{
    protected override async Task<Dictionary<string, McpServerConfigurationItemDto>> HandleAsyncImplementation(
        GetAllMcpServersQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all MCP server configurations");

        var configurations = await repository.GetAllAsync(cancellationToken);

        var result = mapper.MapDomainToDto(configurations);

        logger.LogInformation("Retrieved {Count} MCP server configurations", result.Count);

        return result;
    }
}
