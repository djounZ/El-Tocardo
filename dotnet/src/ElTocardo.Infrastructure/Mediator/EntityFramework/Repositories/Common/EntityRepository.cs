using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.Common.Repositories;
using ElTocardo.Infrastructure.Mediator.EntityFramework.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Mediator.EntityFramework.Repositories.Common;

public abstract class EntityRepository<TEntity, TId, TKey>(
    ApplicationDbContext context,
    DbSet<TEntity> dbSet,
    ILogger<EntityRepository<TEntity,TId,  TKey>> logger) :IEntityRepository<TEntity,TId,TKey> where TEntity: AbstractEntity<TId,TKey>
{
    private static string EntityName => typeof(TEntity).Name;

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting all {@Entities} from database", EntityName);

        return await dbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }


    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting {@Entity} by ID: {Id}", EntityName, id);

        return await GetByIdAsync(id, dbSet, cancellationToken);
    }
    protected abstract Task<TEntity?> GetByIdAsync(TId id, DbSet<TEntity> currentDbSet, CancellationToken cancellationToken = default);
    public async Task<TEntity?> GetByKeyAsync(TKey key, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting {@Entity} by name: {key}", EntityName, key);

        return await GetByKeyAsync(key, dbSet, cancellationToken);
    }

    protected abstract Task<TEntity?> GetByKeyAsync(TKey key, DbSet<TEntity> dbSet,
        CancellationToken cancellationToken = default);


    public async Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Checking if {@Entity} exists: {key}", EntityName, key);

        return await ExistsAsync(key, dbSet, cancellationToken);
    }

    protected abstract  Task<bool> ExistsAsync(TKey key,
        DbSet<TEntity> dbSet, CancellationToken cancellationToken = default);
    public async Task AddAsync(TEntity configuration, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Adding {@Entity}: {key}", EntityName, configuration.GetKey());

        await dbSet.AddAsync(configuration, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public  async Task UpdateAsync(TEntity configuration, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Updating {@Entity}: {key}", EntityName, configuration.GetKey());

        dbSet.Update(configuration);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TKey configuration, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Deleting {@Entity}: {key}", EntityName, configuration);

        var byKeyAsync = await GetByKeyAsync(configuration, cancellationToken);
        dbSet.Remove(byKeyAsync!);
        await context.SaveChangesAsync(cancellationToken);
    }

}
