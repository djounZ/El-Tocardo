using ElTocardo.Application.Mediator.PresetChatInstructionMediator.Commands;
using ElTocardo.Application.Mediator.PresetChatInstructionMediator.Mappers;
using ElTocardo.Domain.Mediator.Common.Interfaces;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Repositories;
using ElTocardo.Domain.Models;

namespace ElTocardo.Application.Mediator.PresetChatInstructionMediator.Handlers.Commands;

public class UpdatePresetChatInstructionCommandHandler(
    IPresetChatInstructionRepository repository,
    PresetChatInstructionDomainUpdateCommandMapper mapper)
    : ICommandHandler<UpdatePresetChatInstructionCommand>
{
    public async Task<VoidResult> HandleAsync(UpdatePresetChatInstructionCommand command, CancellationToken cancellationToken)
    {
        var result = await repository.GetByKeyAsync(command.Name, cancellationToken);
        if (!result.IsSuccess)
        {
            return new KeyNotFoundException($"PresetChatInstruction '{command.Name}' not found.");
        }

        var entity = result.ReadValue();
        mapper.Map(entity, command);
        return await repository.UpdateAsync(entity, cancellationToken);
    }
}
