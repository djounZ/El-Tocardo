using ElTocardo.Application.Mediator.Common.Handlers;
using ElTocardo.Application.Mediator.Common.Handlers.Commands;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Commands;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Handlers.Commands;

public class DeleteMcpServerCommandHandler(
    IMcpServerConfigurationRepository repository,
    ILogger<DeleteMcpServerCommandHandler> logger)
    : DeleteEntityCommandHandler<McpServerConfiguration, string, DeleteMcpServerCommand>(repository, logger);
