using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Repositories;
using ElTocardo.Infrastructure.EntityFramework.Mediator.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.PresetChatOptionsMediator;

public class PresetChatOptionsRepository(
    ApplicationDbContext context,
    ILogger<PresetChatOptionsRepository> logger)
    : DbSetEntityRepository<PresetChatOptions,Guid, string>(context, context.PresetChatOptions, logger), IPresetChatOptionsRepository
{
    protected override async Task<PresetChatOptions?> GetByKeyAsync(string key, DbSet<PresetChatOptions> dbSet, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .FirstOrDefaultAsync(x => x.Name == key, cancellationToken);
    }
}
