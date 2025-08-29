using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.PresetChatInstructionMediator.Commands;
using ElTocardo.Application.Mediator.PresetChatInstructionMediator.Mappers;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Repositories;
using ElTocardo.Domain.Models;

namespace ElTocardo.Application.Mediator.PresetChatInstructionMediator.Handlers.Commands;

public class CreatePresetChatInstructionCommandHandler(
    IPresetChatInstructionRepository repository,
    PresetChatInstructionDomainCreateCommandMapper mapper)
    : ICommandHandler<CreatePresetChatInstructionCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CreatePresetChatInstructionCommand command, CancellationToken cancellationToken)
    {
        var entity = mapper.Map(command);
        return await repository.AddAsync(entity, cancellationToken);
    }
}
