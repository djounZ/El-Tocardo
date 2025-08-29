using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Repositories;
using ElTocardo.Infrastructure.Mediator.EntityFramework.Data;
using ElTocardo.Infrastructure.Mediator.EntityFramework.Repositories.Common;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace ElTocardo.Infrastructure.Mediator.EntityFramework.Repositories;

public class PresetChatInstructionRepository(
    ApplicationDbContext context,
    ILogger<PresetChatInstructionRepository> logger)
    : DbSetEntityRepository<PresetChatInstruction, Guid, string>(context, context.PresetChatInstructions, logger),
        IPresetChatInstructionRepository
{
    protected override async Task<PresetChatInstruction?> GetByKeyAsync(string key, DbSet<PresetChatInstruction> dbSet, CancellationToken cancellationToken = default)
    {
        return await dbSet.FirstOrDefaultAsync(x => x.Name == key, cancellationToken);
    }
}
