using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Repositories;
using ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.PresetChatInstructionMediator;

public class PresetChatInstructionRepository(
    DbContext context,
    DbSet<PresetChatInstruction> dbSet,
    ILogger<PresetChatInstructionRepository> logger)
    : DbSetEntityRepository<PresetChatInstruction, Guid, string>(context, dbSet, logger),
        IPresetChatInstructionRepository
{
    protected override async Task<PresetChatInstruction?> GetByKeyAsync(string key, DbSet<PresetChatInstruction> dbSet, CancellationToken cancellationToken = default)
    {
        return await dbSet.FirstOrDefaultAsync(x => x.Name == key, cancellationToken);
    }
}
