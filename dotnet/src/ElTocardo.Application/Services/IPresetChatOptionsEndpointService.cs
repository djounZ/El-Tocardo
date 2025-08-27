using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Domain.Models;

namespace ElTocardo.Application.Services;

public interface IPresetChatOptionsEndpointService
{
    public Task<Result<List<PresetChatOptionsDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<Result<PresetChatOptionsDto>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    public Task<Result<Guid>> CreateAsync(PresetChatOptionsDto dto, CancellationToken cancellationToken = default);
    public Task<VoidResult> UpdateAsync(string name, PresetChatOptionsDto dto, CancellationToken cancellationToken = default);
    public Task<VoidResult> DeleteAsync(string name, CancellationToken cancellationToken = default);
}
