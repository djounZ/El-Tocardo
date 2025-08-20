using ElTocardo.Application.Dtos.Configuration;

namespace ElTocardo.Application.Services;

public interface IPresetChatOptionsService
{
    public Task<List<PresetChatOptionsDto>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<PresetChatOptionsDto?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    public Task<Guid> CreateAsync(PresetChatOptionsDto dto, CancellationToken cancellationToken = default);
    public Task<bool> UpdateAsync(string name, PresetChatOptionsDto dto, CancellationToken cancellationToken = default);
    public Task<bool> DeleteAsync(string name, CancellationToken cancellationToken = default);
}

// ...existing code...
