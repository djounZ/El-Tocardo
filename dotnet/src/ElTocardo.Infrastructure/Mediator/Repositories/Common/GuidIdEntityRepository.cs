using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Infrastructure.Mediator.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Mediator.Repositories.Common;

public abstract class GuidIdEntityRepository<TEntity, TKey>(
    ApplicationDbContext context,
    DbSet<TEntity> dbSet,
    ILogger<GuidIdEntityRepository<TEntity, TKey>> logger)
    : EntityRepository<TEntity, Guid, TKey>(context, dbSet, logger)
    where TEntity : AbstractEntity<Guid, TKey>
{


    protected override async Task<TEntity?> GetByIdAsync(Guid id, DbSet<TEntity> currentDbSet, CancellationToken cancellationToken = default)
    {
        return await currentDbSet
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
