using ElTocardo.Domain.Mediator.Common.Repositories;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;

namespace ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Repositories;

public interface IPresetChatInstructionRepository : IEntityRepository<PresetChatInstruction, Guid, string>
{
    // Additional custom methods (if needed) can be added here
}
