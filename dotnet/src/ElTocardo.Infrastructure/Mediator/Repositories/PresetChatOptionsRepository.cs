using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Repositories;
using ElTocardo.Infrastructure.Mediator.Data;
using ElTocardo.Infrastructure.Mediator.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Mediator.Repositories;

public class PresetChatOptionsRepository(
    ApplicationDbContext context,
    ILogger<PresetChatOptionsRepository> logger)
    : GuidIdEntityRepository<PresetChatOptions, string>(context, context.PresetChatOptions, logger), IPresetChatOptionsRepository
{
    protected override async Task<PresetChatOptions?> GetByKeyAsync(string key, DbSet<PresetChatOptions> dbSet, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .FirstOrDefaultAsync(x => x.Name == key, cancellationToken);
    }

    protected override async Task<bool> ExistsAsync(string key, DbSet<PresetChatOptions> dbSet, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .AnyAsync(x => x.Name == key, cancellationToken);
    }
}
