using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.Common.Repositories;
using ElTocardo.Domain.Models;
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

    public async Task<Result<IEnumerable<TEntity>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting all {@Entities} from database", EntityName);

        try
        {
            return await dbSet
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all {@Entities} from database", EntityName);
            return ex;
        }
    }


    public async Task<Result<TEntity>> GetByKeyAsync(TKey key, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting {@Entity} by name: {key}", EntityName, key);

        try
        {
            var byKeyAsync = await GetByKeyAsync(key, dbSet, cancellationToken);
            if (byKeyAsync is null)
            {
                logger.LogError("Key {key} not found",key);
                return new KeyNotFoundException($"Key {key} not found");
            }

            return byKeyAsync;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting {@Entity} by name: {key}", EntityName, key);
            return ex;
        }
    }

    protected abstract Task<TEntity?> GetByKeyAsync(TKey key, DbSet<TEntity> dbSet,
        CancellationToken cancellationToken = default);


    public async Task<VoidResult> AddAsync(TEntity configuration, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Adding {@Entity}: {key}", EntityName, configuration.GetKey());

        try
        {
            await dbSet.AddAsync(configuration, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return VoidResult.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding {@Entity}: {key}", EntityName, configuration.GetKey());
            return ex;
        }
    }

    public  async Task<VoidResult> UpdateAsync(TEntity configuration, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Updating {@Entity}: {key}", EntityName, configuration.GetKey());
        try
        {
            dbSet.Update(configuration);
            await context.SaveChangesAsync(cancellationToken);
            return VoidResult.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting {@Entity}: {key}", EntityName, configuration.GetKey());
            return ex;
        }
    }

    public async Task<VoidResult> DeleteAsync(TKey configuration, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Deleting {@Entity}: {key}", EntityName, configuration);

        var byKeyAsync = await GetByKeyAsync(configuration, cancellationToken);

        if (!byKeyAsync.IsSuccess)
        {
            return byKeyAsync;
        }

        try
        {

            dbSet.Remove(byKeyAsync.ReadValue());
            await context.SaveChangesAsync(cancellationToken);
            return  VoidResult.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting {@Entity}: {key}", EntityName, configuration);
            return ex;
        }
    }

}
