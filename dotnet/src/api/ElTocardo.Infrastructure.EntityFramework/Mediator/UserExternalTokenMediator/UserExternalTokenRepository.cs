using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Repositories;
using ElTocardo.Domain.Models;
using ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.UserExternalTokenMediator;

public class UserExternalTokenRepository(
    DbContext context,
    DbSet<UserExternalToken> dbSet,
    UserExternalTokenProtector userExternalTokenProtector,
    ILogger<UserExternalTokenRepository> logger)
    : DbSetEntityRepository<UserExternalToken, Guid, UserExternalTokenKey>(context, dbSet, logger), IUserExternalTokenRepository
{
    protected override async Task<UserExternalToken?> GetByKeyAsync(UserExternalTokenKey key, DbSet<UserExternalToken> dbSet, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .FirstOrDefaultAsync(x => x.UserId == key.UserId && x.Provider == key.Provider, cancellationToken);
    }

    public override Task<Result<Guid>> AddAsync(UserExternalToken entity, CancellationToken cancellationToken = default)
    {
        return base.AddAsync(ProtectedEntity(entity), cancellationToken);
    }

    public override async Task<VoidResult> UpdateAsync(UserExternalToken entity,
        CancellationToken cancellationToken = default)
    {
        return await base.UpdateAsync(ProtectedEntity(entity), cancellationToken);
    }

    private UserExternalToken ProtectedEntity(UserExternalToken entity)
    {
        return new UserExternalToken(entity.UserId, entity.Provider, userExternalTokenProtector.Protect(entity.Value));
    }
}
