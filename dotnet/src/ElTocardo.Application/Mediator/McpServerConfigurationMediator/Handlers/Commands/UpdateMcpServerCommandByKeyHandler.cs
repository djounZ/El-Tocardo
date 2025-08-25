using ElTocardo.Application.Mediator.Common.Handlers.Commands;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Commands;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Mappers;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Handlers.Commands;

public class UpdateMcpServerCommandByKeyHandler(
    IMcpServerConfigurationRepository repository,
    ILogger<UpdateMcpServerCommandByKeyHandler> logger,
    IValidator<UpdateMcpServerCommand> validator,
    McpServerConfigurationDomainUpdateCommandMapper mapper)
    : UpdateEntityCommandByKeyHandler<McpServerConfiguration, Guid, string, UpdateMcpServerCommand>(repository, logger, validator,
        mapper);
