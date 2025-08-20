using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Mappers;

public class PresetChatOptionsDomainGetAllDtoMapper(PresetChatOptionsDomainGetDtoMapper mapper)
    : AbstractDomainGetAllDtoMapper<PresetChatOptions, string, List<PresetChatOptionsDto>>
{
    public override List<PresetChatOptionsDto> MapDomainToDto(IEnumerable<PresetChatOptions> entities)
    {
        return [.. entities.Select(mapper.MapDomainToDto)];
    }
}
