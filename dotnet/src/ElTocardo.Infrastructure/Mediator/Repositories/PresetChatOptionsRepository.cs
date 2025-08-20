using ElTocardo.Domain.Entities;
using ElTocardo.Domain.Repositories;
using ElTocardo.Infrastructure.Mediator.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Mediator.Repositories;

public class PresetChatOptionsRepository(
    ApplicationDbContext context,
    ILogger<PresetChatOptionsRepository> logger)
    : IPresetChatOptionsRepository
{
    public async Task<List<PresetChatOptions>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching all PresetChatOptions from database");
        var result = await context.PresetChatOptions.ToListAsync(cancellationToken);
        logger.LogDebug("Fetched {Count} PresetChatOptions", result.Count);
        return result;
    }

    public async Task<PresetChatOptions?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching PresetChatOptions with Name: {Name}", name);
        var entity = await context.PresetChatOptions.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        if (entity == null)
        {
            logger.LogWarning("PresetChatOptions with Name: {Name} not found", name);
        }
        else
        {
            logger.LogDebug("PresetChatOptions with Name: {Name} found (Id: {Id})", name, entity.Id);
        }

        return entity;
    }

    public async Task AddAsync(PresetChatOptions preset, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Adding new PresetChatOptions with Name: {Name}", preset.Name);
        await context.PresetChatOptions.AddAsync(preset, cancellationToken);
    }

    public async Task UpdateAsync(PresetChatOptions preset, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating PresetChatOptions with Id: {Id}, Name: {Name}", preset.Id, preset.Name);
        context.PresetChatOptions.Update(preset);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(PresetChatOptions preset, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting PresetChatOptions with Id: {Id}, Name: {Name}", preset.Id, preset.Name);
        context.PresetChatOptions.Remove(preset);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Saving changes to database for PresetChatOptions");
        await context.SaveChangesAsync(cancellationToken);
    }
}
