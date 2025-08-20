using ElTocardo.Application.Commands.McpServerConfiguration;
using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Handlers.McpServerConfiguration;

public class UpdateMcpServerCommandHandler(
    IMcpServerConfigurationRepository repository,
    ILogger<UpdateMcpServerCommandHandler> logger,
    IValidator<UpdateMcpServerCommand> validator)
    : CommandHandlerBase<UpdateMcpServerCommand>(logger)
{
    protected override async Task HandleAsyncImplementation(UpdateMcpServerCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating MCP server configuration: {ServerName}", command.ServerName);

        // Validate command
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        // Get existing configuration
        var configuration = await repository.GetByNameAsync(command.ServerName, cancellationToken);

        // Update configuration
        configuration!.UpdateConfiguration(
            command.Category,
            command.Command,
            command.Arguments,
            command.EnvironmentVariables,
            command.Endpoint,
            command.TransportType);

        // Save changes
        await repository.UpdateAsync(configuration, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("MCP server configuration updated successfully: {ServerName}", command.ServerName);
    }
}
