
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Dtos.PresetChatInstruction;
using ElTocardo.Application.Mediator.PresetChatInstructionMediator.Queries;
using ElTocardo.Application.Mediator.PresetChatInstructionMediator.Mappers;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Repositories;
using ElTocardo.Domain.Models;

namespace ElTocardo.Application.Mediator.PresetChatInstructionMediator.Handlers.Queries;

public class GetPresetChatInstructionByNameQueryHandler(
    IPresetChatInstructionRepository repository,
    PresetChatInstructionDomainGetDtoMapper mapper)
    : IQueryHandler<GetPresetChatInstructionByNameQuery, PresetChatInstructionDto>
{
    public async Task<Result<PresetChatInstructionDto>> HandleAsync(GetPresetChatInstructionByNameQuery query, CancellationToken cancellationToken)
    {
        var result = await repository.GetByKeyAsync(query.Name, cancellationToken);
        if (!result.IsSuccess)
        {
            return new KeyNotFoundException($"PresetChatInstruction '{query.Name}' not found.");
        }

        return mapper.Map(result.ReadValue());
    }
}
