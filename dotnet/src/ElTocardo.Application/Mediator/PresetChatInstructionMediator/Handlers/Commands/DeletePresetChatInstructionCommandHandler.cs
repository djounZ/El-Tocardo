
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.PresetChatInstructionMediator.Commands;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Repositories;
using ElTocardo.Domain.Models;

namespace ElTocardo.Application.Mediator.PresetChatInstructionMediator.Handlers.Commands;

public class DeletePresetChatInstructionCommandHandler(IPresetChatInstructionRepository repository)
    : ICommandHandler<DeletePresetChatInstructionCommand>
{
    public async Task<VoidResult> HandleAsync(DeletePresetChatInstructionCommand command, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(command.Name, cancellationToken);
    }
}
