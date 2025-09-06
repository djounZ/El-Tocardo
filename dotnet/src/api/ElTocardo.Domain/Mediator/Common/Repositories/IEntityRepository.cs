using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Models;

namespace ElTocardo.Domain.Mediator.Common.Repositories;

public interface IEntityRepository<TEntity,  TId, in TKey> where TEntity: IEntity<TId,TKey>
{
    public Task<Result<IEnumerable<TEntity>>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<Result<TEntity>> GetByKeyAsync(TKey key, CancellationToken cancellationToken = default);
    public Task<Result<TId>> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    public Task<VoidResult> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    public Task<VoidResult> DeleteAsync(TKey key, CancellationToken cancellationToken = default);

}
