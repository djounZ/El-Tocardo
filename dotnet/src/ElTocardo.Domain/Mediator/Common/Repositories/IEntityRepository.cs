using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Domain.Models;

namespace ElTocardo.Domain.Mediator.Common.Repositories;

public interface IEntityRepository<TEntity, in TId, in TKey> where TEntity: IEntity<TId,TKey>
{
    public Task<Result<IEnumerable<TEntity>>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<Result<TEntity>> GetByKeyAsync(TKey key, CancellationToken cancellationToken = default);
    public Task<VoidResult> AddAsync(TEntity configuration, CancellationToken cancellationToken = default);
    public Task<VoidResult> UpdateAsync(TEntity configuration, CancellationToken cancellationToken = default);
    public Task<VoidResult> DeleteAsync(TKey configuration, CancellationToken cancellationToken = default);

}
