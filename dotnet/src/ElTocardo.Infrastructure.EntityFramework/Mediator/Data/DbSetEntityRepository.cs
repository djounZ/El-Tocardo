using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.Common.Repositories;
using ElTocardo.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.Data;

public abstract class DbSetEntityRepository<TEntity, TId, TKey>(
    ApplicationDbContext context,
    DbSet<TEntity> dbSet,
    ILogger<DbSetEntityRepository<TEntity,TId,  TKey>> logger) :IEntityRepository<TEntity,TId,TKey> where TEntity: AbstractEntity<TId,TKey>
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


    public async Task<Result<TId>> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Adding {@Entity}: {key}", EntityName, entity.GetKey());

        try
        {
            await dbSet.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding {@Entity}: {key}", EntityName, entity.GetKey());
            return ex;
        }
    }

    public  async Task<VoidResult> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Updating {@Entity}: {key}", EntityName, entity.GetKey());
        try
        {
            dbSet.Update(entity);
            await context.SaveChangesAsync(cancellationToken);
            return VoidResult.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting {@Entity}: {key}", EntityName, entity.GetKey());
            return ex;
        }
    }

    public async Task<VoidResult> DeleteAsync(TKey key, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Deleting {@Entity}: {key}", EntityName, key);

        var byKeyAsync = await GetByKeyAsync(key, cancellationToken);

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
            logger.LogError(ex, "Error deleting {@Entity}: {key}", EntityName, key);
            return ex;
        }
    }

}
