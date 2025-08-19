using ElTocardo.Application.Commands.McpServerConfiguration;
using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Application.Common.Models;
using ElTocardo.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Handlers.McpServerConfiguration;

public class UpdateMcpServerCommandHandler(
    IMcpServerConfigurationRepository repository,
    ILogger<UpdateMcpServerCommandHandler> logger,
    IValidator<UpdateMcpServerCommand> validator)
    : ICommandHandler<UpdateMcpServerCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateMcpServerCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Updating MCP server configuration: {ServerName}", command.ServerName);

            // Validate command
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                logger.LogWarning("Validation failed for MCP server update: {ServerName}. Errors: {Errors}",
                    command.ServerName, errors);
                return Result.Failure($"Validation failed: {errors}");
            }

            // Get existing configuration
            var configuration = await repository.GetByNameAsync(command.ServerName, cancellationToken);
            if (configuration == null)
            {
                logger.LogWarning("MCP server not found: {ServerName}", command.ServerName);
                return Result.Failure($"Server '{command.ServerName}' not found");
            }

            // Update configuration
            configuration.UpdateConfiguration(
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

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update MCP server configuration: {ServerName}", command.ServerName);
            return Result.Failure("Failed to update MCP server configuration", ex);
        }
    }
}
