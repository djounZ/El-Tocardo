using ElTocardo.Application.Commands.McpServerConfiguration;
using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Handlers.McpServerConfiguration;

public class DeleteMcpServerCommandHandler(
    IMcpServerConfigurationRepository repository,
    ILogger<DeleteMcpServerCommandHandler> logger)
    : CommandHandlerBase<DeleteMcpServerCommand>(logger)
{
    protected override async Task HandleAsyncImplementation(DeleteMcpServerCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting MCP server configuration: {ServerName}", command.ServerName);

        // Get existing configuration
        var configuration = await repository.GetByNameAsync(command.ServerName, cancellationToken);
        if (configuration == null)
        {
            logger.LogWarning("MCP server not found: {ServerName}", command.ServerName);
            return;
        }

        // Delete configuration
        await repository.DeleteAsync(configuration, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("MCP server configuration deleted successfully: {ServerName}", command.ServerName);
    }
}
