using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Dtos.ModelContextProtocol.Core;
using ElTocardo.Application.Mediator.Common.Handlers.Queries;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Mappers;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Queries;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Handlers.Queries;

public class GetMcpServerByNameQueryHandler(
    IMcpServerConfigurationRepository repository,
    ILogger<GetMcpServerByNameQueryHandler> logger,
    McpServerConfigurationDomainGetDtoMapper mapper)
    : GetEntityByKeyQueryHandler<McpServerConfiguration, Guid,string, GetMcpServerByNameQuery,
        McpServerConfigurationItemDto>(repository, logger, mapper);
