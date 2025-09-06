using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Repositories;
using ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.McpServerConfigurationMediator;

public class McpServerConfigurationRepository(
    DbContext context,
    DbSet<McpServerConfiguration> dbSet,
    ILogger<McpServerConfigurationRepository> logger)
    : DbSetEntityRepository<McpServerConfiguration,Guid, string>(context, dbSet, logger), IMcpServerConfigurationRepository
{
    protected override async Task<McpServerConfiguration?> GetByKeyAsync(string key, DbSet<McpServerConfiguration> dbSet, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .FirstOrDefaultAsync(x => x.ServerName == key, cancellationToken);
    }

}
