using ElTocardo.Domain.Mediator.Common.Entities;
using ElTocardo.Infrastructure.Mediator.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Mediator.Repositories.Common;

public abstract class StringIdEntityRepository<TEntity, TKey>(
    ApplicationDbContext context,
    DbSet<TEntity> dbSet,
    ILogger<StringIdEntityRepository<TEntity, TKey>> logger)
    : EntityRepository<TEntity, string, TKey>(context, dbSet, logger)
    where TEntity : AbstractEntity<string, TKey>
{


    protected override async Task<TEntity?> GetByIdAsync(string id, DbSet<TEntity> currentDbSet, CancellationToken cancellationToken = default)
    {
        return await currentDbSet
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
