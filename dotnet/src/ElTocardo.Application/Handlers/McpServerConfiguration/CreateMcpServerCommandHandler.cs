using ElTocardo.Application.Commands.McpServerConfiguration;
using ElTocardo.Application.Common.Interfaces;
using ElTocardo.Application.Common.Models;
using ElTocardo.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Handlers.McpServerConfiguration;

public class CreateMcpServerCommandHandler(
    IMcpServerConfigurationRepository repository,
    ILogger<CreateMcpServerCommandHandler> logger,
    IValidator<CreateMcpServerCommand> validator)
    : ICommandHandler<CreateMcpServerCommand, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(CreateMcpServerCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Creating MCP server configuration: {ServerName}", command.ServerName);

            // Validate command
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                logger.LogWarning("Validation failed for MCP server creation: {ServerName}. Errors: {Errors}",
                    command.ServerName, errors);
                return Result<Guid>.Failure($"Validation failed: {errors}");
            }

            // Check if server already exists
            if (await repository.ExistsAsync(command.ServerName, cancellationToken))
            {
                logger.LogWarning("MCP server already exists: {ServerName}", command.ServerName);
                return Result<Guid>.Failure($"Server '{command.ServerName}' already exists");
            }

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

            return Result<Guid>.Success(configuration.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create MCP server configuration: {ServerName}", command.ServerName);
            return Result<Guid>.Failure("Failed to create MCP server configuration", ex);
        }
    }
}
