using ElTocardo.Application.Mediator.Common.Handlers;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Commands;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Mappers;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Handlers.Commands;

public class UpdateMcpServerCommandHandler(
    IMcpServerConfigurationRepository repository,
    ILogger<UpdateMcpServerCommandHandler> logger,
    IValidator<UpdateMcpServerCommand> validator,
    McpServerConfigurationDomainCommandMapper mapper)
    : CommandHandlerBase<UpdateMcpServerCommand>(logger)
{
    protected override async Task HandleAsyncImplementation(UpdateMcpServerCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating MCP server configuration: {ServerName}", command.ServerName);

        // Validate command
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        // Get existing configuration
        var configuration = await repository.GetByKeyAsync(command.ServerName, cancellationToken);

        // Update configuration
        mapper.UpdateFromCommand(configuration!, command);

        // Save changes
        await repository.UpdateAsync(configuration!, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("MCP server configuration updated successfully: {ServerName}", command.ServerName);
    }
}
