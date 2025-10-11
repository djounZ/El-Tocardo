using System.Linq.Expressions;
using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Mediator.Common.Repositories;
using ElTocardo.Domain.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ElTocardo.Infrastructure.Mediator.MongoDb.Repositories.Common;

public abstract class MongoCollectionRepository<TEntity, TId, TKey>(ILogger<MongoCollectionRepository<TEntity, TId, TKey>> logger, IMongoDatabase mongoDatabase)  :IEntityRepository<TEntity,TId,TKey> where TEntity: AbstractEntity<TId,TKey>
{
    protected readonly IMongoCollection<TEntity> EntityCollection = mongoDatabase.GetCollection<TEntity>(typeof(TEntity).Name);

    private static string EntityName => typeof(TEntity).Name;
    public async Task<Result<IEnumerable<TEntity>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting all {EntityName}", EntityName);
        try
        {

            var entities = await EntityCollection.Find(_ => true).ToListAsync(cancellationToken);
            logger.LogDebug("Retrieved {Count} {EntityName}", entities.Count, EntityName);
            return entities;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting {EntityName}", EntityName);
            return ex;
        }
    }

    protected abstract Expression<Func<TEntity, bool>> GetByKeySelector(TKey id);

    public async Task<Result<TEntity>> GetByKeyAsync(TKey key, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogDebug("Getting {EntityName} by key: {Key}",EntityName, key);
            var selector = GetByKeySelector(key);
            var conversation = await EntityCollection.Find(selector).FirstOrDefaultAsync(cancellationToken);
            if (conversation == null)
            {
                return new KeyNotFoundException($"Key {key} not found");
            }
            return conversation;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting {EntityName} by key {Key}", EntityName, key);
            return ex;
        }
    }

    public async Task<Result<TId>> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {

            logger.LogDebug("Adding {EntityName} with ID: {Id}", EntityName, entity.Id);
            await EntityCollection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            logger.LogDebug("Successfully added {EntityName} with ID: {Id}", EntityName, entity.Id);
            return entity.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding {EntityName}", EntityName);
            return ex;
        }
    }

    protected abstract FilterDefinition<TEntity> GetUpdateFilter(TEntity entity);
    public async Task<VoidResult> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogDebug("Updating {EntityName} with ID: {Id}", EntityName, entity.Id);
            var filter =  GetUpdateFilter(entity);
            var result = await EntityCollection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);

            if (result.MatchedCount == 0)
            {
                logger.LogWarning("No {EntityName} found with ID: {Id} for update", EntityName, entity.Id);
                return new InvalidOperationException($"{EntityName} with ID {entity.Id} not found for update");
            }

            logger.LogDebug("Successfully updated {EntityName} with ID: {Id}", EntityName, entity.Id);
            return VoidResult.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating {EntityName}",  EntityName);
            return ex;
        }
    }

    protected abstract FilterDefinition<TEntity> GetDeleteFilter(TKey key);
    public async Task<VoidResult> DeleteAsync(TKey key, CancellationToken cancellationToken = default)
    {
        try
        {

            logger.LogDebug("Deleting {EntityName} with ID: {Id}", EntityName, key);
            var filter = GetDeleteFilter(key);
            var result = await EntityCollection.DeleteOneAsync(filter, cancellationToken);

            if (result.DeletedCount == 0)
            {
                logger.LogWarning("No {EntityName} found with ID: {Id} for deletion",EntityName, key);
                return new InvalidOperationException($"{EntityName} with ID {key} not found for deletion");
            }

            logger.LogDebug("Successfully deleted {EntityName} with ID: {Id}",EntityName, key);
            return VoidResult.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting conversation");
            return ex;
        }
    }
}
