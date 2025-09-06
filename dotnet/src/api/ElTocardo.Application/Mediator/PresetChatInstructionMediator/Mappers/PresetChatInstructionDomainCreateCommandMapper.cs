using ElTocardo.Application.Mediator.PresetChatInstructionMediator.Commands;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;

namespace ElTocardo.Application.Mediator.PresetChatInstructionMediator.Mappers;

public class PresetChatInstructionDomainCreateCommandMapper
{
    public PresetChatInstruction Map(CreatePresetChatInstructionCommand command)
    {
        return new PresetChatInstruction(command.Name, command.Description, command.ContentType, command.Content);
    }
}
