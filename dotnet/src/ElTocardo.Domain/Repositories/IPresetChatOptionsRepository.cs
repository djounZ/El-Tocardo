using ElTocardo.Domain.Entities;

namespace ElTocardo.Domain.Repositories;

public interface IPresetChatOptionsRepository
{
    Task<IEnumerable<PresetChatOptions>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PresetChatOptions?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(PresetChatOptions preset, CancellationToken cancellationToken = default);
    Task UpdateAsync(PresetChatOptions preset, CancellationToken cancellationToken = default);
    Task DeleteAsync(PresetChatOptions preset, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
