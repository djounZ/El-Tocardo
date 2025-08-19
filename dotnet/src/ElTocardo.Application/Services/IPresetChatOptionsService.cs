using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Domain.Entities;
using ElTocardo.Domain.Repositories;
using System.Text.Json;

namespace ElTocardo.Application.Services;

public interface IPresetChatOptionsService
{
    Task<IEnumerable<PresetChatOptionsDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PresetChatOptionsDto?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(PresetChatOptionsDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(string name, PresetChatOptionsDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string name, CancellationToken cancellationToken = default);
}

// ...existing code...
