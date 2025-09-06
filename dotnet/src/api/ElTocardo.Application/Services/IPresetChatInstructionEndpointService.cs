using ElTocardo.Application.Dtos.PresetChatInstruction;
using ElTocardo.Domain.Models;

namespace ElTocardo.Application.Services;

public interface IPresetChatInstructionEndpointService
{
    public Task<Result<Guid>> CreateAsync(string name, string description, string contentType, string content, CancellationToken cancellationToken = default);
    public Task<VoidResult> UpdateAsync(string name, string description, string contentType, string content, CancellationToken cancellationToken = default);
    public Task<VoidResult> DeleteAsync(string name, CancellationToken cancellationToken = default);
    public Task<Result<IList<PresetChatInstructionDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<Result<PresetChatInstructionDto>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
