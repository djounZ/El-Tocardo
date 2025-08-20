using ElTocardo.Domain.Entities;

namespace ElTocardo.Domain.Repositories;

public interface IPresetChatOptionsRepository
{
    public Task<List<PresetChatOptions>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<PresetChatOptions?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    public Task AddAsync(PresetChatOptions preset, CancellationToken cancellationToken = default);
    public Task UpdateAsync(PresetChatOptions preset, CancellationToken cancellationToken = default);
    public Task DeleteAsync(PresetChatOptions preset, CancellationToken cancellationToken = default);
    public Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
