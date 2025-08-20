using ElTocardo.Domain.Entities;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using ElTocardo.Domain.Repositories;
using ElTocardo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Repositories;

public class McpServerConfigurationRepository(
    ApplicationDbContext context,
    ILogger<McpServerConfigurationRepository> logger)
    : IMcpServerConfigurationRepository
{
    public async Task<IEnumerable<McpServerConfiguration>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting all MCP server configurations from database");

        return await context.McpServerConfigurations
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<McpServerConfiguration?> GetByNameAsync(string serverName, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting MCP server configuration by name: {ServerName}", serverName);

        return await context.McpServerConfigurations
            .FirstOrDefaultAsync(x => x.ServerName == serverName, cancellationToken);
    }

    public async Task<McpServerConfiguration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting MCP server configuration by ID: {Id}", id);

        return await context.McpServerConfigurations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string serverName, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Checking if MCP server configuration exists: {ServerName}", serverName);

        return await context.McpServerConfigurations
            .AnyAsync(x => x.ServerName == serverName, cancellationToken);
    }

    public async Task AddAsync(McpServerConfiguration configuration, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Adding MCP server configuration: {ServerName}", configuration.ServerName);

        await context.McpServerConfigurations.AddAsync(configuration, cancellationToken);
    }

    public Task UpdateAsync(McpServerConfiguration configuration, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Updating MCP server configuration: {ServerName}", configuration.ServerName);

        context.McpServerConfigurations.Update(configuration);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(McpServerConfiguration configuration, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Deleting MCP server configuration: {ServerName}", configuration.ServerName);

        context.McpServerConfigurations.Remove(configuration);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Saving changes to database");

        await context.SaveChangesAsync(cancellationToken);
    }
}
