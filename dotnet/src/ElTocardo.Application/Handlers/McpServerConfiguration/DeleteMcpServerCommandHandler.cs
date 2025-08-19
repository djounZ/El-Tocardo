using ElTocardo.Application.Commands.McpServerConfiguration;
using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Application.Common.Models;
using ElTocardo.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Handlers.McpServerConfiguration;

public class DeleteMcpServerCommandHandler(
    IMcpServerConfigurationRepository repository,
    ILogger<DeleteMcpServerCommandHandler> logger)
    : ICommandHandler<DeleteMcpServerCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteMcpServerCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Deleting MCP server configuration: {ServerName}", command.ServerName);

            // Get existing configuration
            var configuration = await repository.GetByNameAsync(command.ServerName, cancellationToken);
            if (configuration == null)
            {
                logger.LogWarning("MCP server not found: {ServerName}", command.ServerName);
                return Result.Failure($"Server '{command.ServerName}' not found");
            }

            // Delete configuration
            await repository.DeleteAsync(configuration, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("MCP server configuration deleted successfully: {ServerName}", command.ServerName);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete MCP server configuration: {ServerName}", command.ServerName);
            return Result.Failure("Failed to delete MCP server configuration", ex);
        }
    }
}
