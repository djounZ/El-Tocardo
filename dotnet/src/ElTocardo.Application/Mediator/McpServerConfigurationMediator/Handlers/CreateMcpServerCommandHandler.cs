using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Commands;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Mappers;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Handlers;

public class CreateMcpServerCommandHandler(
    IMcpServerConfigurationRepository repository,
    ILogger<CreateMcpServerCommandHandler> logger,
    IValidator<CreateMcpServerCommand> validator,
    McpServerConfigurationDomainCommandMapper commandMapper)
    : CommandHandlerBase<CreateMcpServerCommand, Guid>(logger)
{
    protected override async Task<Guid> HandleAsyncImplementation(CreateMcpServerCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating MCP server configuration: {ServerName}", command.ServerName);

        // Validate command
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        // Create domain entity
        var configuration = commandMapper.CreateFromCommand(command);

        // Add to repository
        await repository.AddAsync(configuration, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("MCP server configuration created successfully: {ServerName} with ID: {Id}",
            command.ServerName, configuration.Id);

        return configuration.Id;
    }
}
