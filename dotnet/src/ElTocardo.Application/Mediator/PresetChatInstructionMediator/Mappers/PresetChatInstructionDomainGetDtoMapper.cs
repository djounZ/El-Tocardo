using ElTocardo.Application.Dtos.PresetChatInstruction;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;

namespace ElTocardo.Application.Mediator.PresetChatInstructionMediator.Mappers;

public class PresetChatInstructionDomainGetDtoMapper
{
    public PresetChatInstructionDto Map(PresetChatInstruction entity)
    {
        return new PresetChatInstructionDto(entity.Name, entity.Description, entity.ContentType, entity.Content);
    }
}
