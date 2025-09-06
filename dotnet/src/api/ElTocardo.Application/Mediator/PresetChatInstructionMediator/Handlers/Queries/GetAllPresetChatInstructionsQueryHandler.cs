using ElTocardo.Application.Dtos.PresetChatInstruction;
using ElTocardo.Application.Mediator.PresetChatInstructionMediator.Queries;
using ElTocardo.Application.Mediator.PresetChatInstructionMediator.Mappers;
using ElTocardo.Domain.Mediator.Common.Interfaces;
using ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Repositories;
using ElTocardo.Domain.Models;

namespace ElTocardo.Application.Mediator.PresetChatInstructionMediator.Handlers.Queries;

public class GetAllPresetChatInstructionsQueryHandler(
    IPresetChatInstructionRepository repository,
    PresetChatInstructionDomainGetAllDtoMapper mapper)
    : IQueryHandler<GetAllPresetChatInstructionsQuery, List<PresetChatInstructionDto>>
{
    public async Task<Result<List<PresetChatInstructionDto>>> HandleAsync(GetAllPresetChatInstructionsQuery query, CancellationToken cancellationToken)
    {
        var result = await repository.GetAllAsync(cancellationToken);
        if (!result.IsSuccess)
        {
            return new KeyNotFoundException("PresetChatInstructions not found or repository error.");
        }

        return mapper.Map(result.ReadValue());
    }
}
