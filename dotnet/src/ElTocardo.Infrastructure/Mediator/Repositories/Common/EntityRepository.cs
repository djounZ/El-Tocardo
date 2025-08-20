using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.Common.Repositories;
using ElTocardo.Infrastructure.Mediator.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Mediator.Repositories.Common;

public class EntityRepository<TEntity,  TKey>(
    ApplicationDbContext context,
    DbSet<TEntity> dbSet,
    ILogger<EntityRepository<TEntity,  TKey>> logger) :IEntityRepository<TEntity,TKey> where TEntity: AbstractEntity<TKey>
{

    private static string EntityName => typeof(TEntity).Name;

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting all {@Entities} from database", EntityName);

        return await dbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByKeyAsync(TKey key, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting {@Entity} by name: {key}", EntityName, key);

        return await dbSet
            .FirstOrDefaultAsync(x => Equals(x.GetKey(), key), cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting {@Entity} by ID: {Id}", EntityName, id);

        return await dbSet
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Checking if {@Entity} exists: {key}", EntityName, key);

        return await dbSet
            .AnyAsync(x => Equals(x.GetKey(), key), cancellationToken);
    }

    public async Task AddAsync(TEntity configuration, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Adding {@Entity}: {key}", EntityName, configuration.GetKey());

        await dbSet.AddAsync(configuration, cancellationToken);
    }

    public Task UpdateAsync(TEntity configuration, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Updating {@Entity}: {key}", EntityName, configuration.GetKey());

        dbSet.Update(configuration);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TEntity configuration, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Deleting {@Entity}: {key}", EntityName, configuration.GetKey());

        dbSet.Remove(configuration);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Saving changes to database");

        await context.SaveChangesAsync(cancellationToken);
    }
}
