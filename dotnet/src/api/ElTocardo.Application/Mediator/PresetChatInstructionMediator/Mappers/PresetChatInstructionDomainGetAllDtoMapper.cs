using ElTocardo.Application.Dtos.PresetChatInstruction;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;

namespace ElTocardo.Application.Mediator.PresetChatInstructionMediator.Mappers;

public class PresetChatInstructionDomainGetAllDtoMapper
{
    public List<PresetChatInstructionDto> Map(IEnumerable<PresetChatInstruction> entities)
    {
        return entities.Select(e => new PresetChatInstructionDto(e.Name, e.Description, e.ContentType, e.Content)).ToList();
    }
}
