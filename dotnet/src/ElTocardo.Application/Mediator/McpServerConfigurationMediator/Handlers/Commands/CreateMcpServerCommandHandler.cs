using ElTocardo.Application.Mediator.Common.Commands;
using ElTocardo.Application.Mediator.Common.Handlers.Commands;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Commands;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Mappers;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Handlers.Commands;

public class CreateMcpServerCommandHandler(
    IMcpServerConfigurationRepository repository,
    ILogger<CreateMcpServerCommandHandler> logger,
    IValidator<CreateMcpServerCommand> validator,
    McpServerConfigurationDomainCreateCommandMapper createCommandMapper)
    : CreateEntityCommandHandler<McpServerConfiguration, string, CreateMcpServerCommand>(repository, logger, validator, createCommandMapper);
