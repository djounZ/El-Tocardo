using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using ElTocardo.Infrastructure.Mediator.Data;
using ElTocardo.Infrastructure.Mediator.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Mediator.Repositories;

public class McpServerConfigurationRepository(
    ApplicationDbContext context,
    ILogger<McpServerConfigurationRepository> logger)
    : EntityRepository<McpServerConfiguration, string>(context, context.McpServerConfigurations, logger), IMcpServerConfigurationRepository
{
    protected override async Task<McpServerConfiguration?> GetByKeyAsync(string key, DbSet<McpServerConfiguration> dbSet, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .FirstOrDefaultAsync(x => x.ServerName == key, cancellationToken);
    }

    protected  override  async Task<bool> ExistsAsync(string key, DbSet<McpServerConfiguration> dbSet, CancellationToken cancellationToken = default)
    {
        return  await dbSet
            .AnyAsync(x => x.ServerName == key, cancellationToken);
    }
}
