using ElTocardo.Application.Commands.McpServerConfiguration;
using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Handlers.McpServerConfiguration;

public class CreateMcpServerCommandHandler(
    IMcpServerConfigurationRepository repository,
    ILogger<CreateMcpServerCommandHandler> logger,
    IValidator<CreateMcpServerCommand> validator)
    : CommandHandlerBase<CreateMcpServerCommand, Guid>(logger)
{
    protected override async Task<Guid> HandleAsyncImplementation(CreateMcpServerCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating MCP server configuration: {ServerName}", command.ServerName);

        // Validate command
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        // Create domain entity
        var configuration = new Domain.Entities.McpServerConfiguration(
            command.ServerName,
            command.Category,
            command.Command,
            command.Arguments,
            command.EnvironmentVariables,
            command.Endpoint,
            command.TransportType);

        // Add to repository
        await repository.AddAsync(configuration, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("MCP server configuration created successfully: {ServerName} with ID: {Id}",
            command.ServerName, configuration.Id);

        return configuration.Id;
    }
}
