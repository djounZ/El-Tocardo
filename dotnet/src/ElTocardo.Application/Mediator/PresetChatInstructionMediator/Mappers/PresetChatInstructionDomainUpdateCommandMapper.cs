using ElTocardo.Application.Mediator.PresetChatInstructionMediator.Commands;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;

namespace ElTocardo.Application.Mediator.PresetChatInstructionMediator.Mappers;

public class PresetChatInstructionDomainUpdateCommandMapper
{
    public void Map(PresetChatInstruction entity, UpdatePresetChatInstructionCommand command)
    {
        entity.Update(command.Description, command.ContentType, command.Content);
    }
}
