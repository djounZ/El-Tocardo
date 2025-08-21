using ElTocardo.Domain.Mediator.Common.Entities;

namespace ElTocardo.Domain.Mediator.Common.Repositories;

public interface IEntityRepository<TEntity, in TId, in TKey> where TEntity: IEntity<TId,TKey>
{
    public Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<TEntity?> GetByKeyAsync(TKey key, CancellationToken cancellationToken = default);
    public Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    public Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default);
    public Task AddAsync(TEntity configuration, CancellationToken cancellationToken = default);
    public Task UpdateAsync(TEntity configuration, CancellationToken cancellationToken = default);
    public Task DeleteAsync(TEntity configuration, CancellationToken cancellationToken = default);
    public Task SaveChangesAsync(CancellationToken cancellationToken = default);

}
