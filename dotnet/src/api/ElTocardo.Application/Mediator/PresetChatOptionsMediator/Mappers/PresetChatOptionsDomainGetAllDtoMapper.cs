using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Domain.Mediator.Mappers;
using ElTocardo.Domain.Mediator.PresetChatOptionsMediator.Entities;

namespace ElTocardo.Application.Mediator.PresetChatOptionsMediator.Mappers;

public class PresetChatOptionsDomainGetAllDtoMapper(PresetChatOptionsDomainGetDtoMapper mapper)
    : AbstractDomainGetAllDtoMapper<PresetChatOptions, Guid,string, List<PresetChatOptionsDto>>
{
    public override List<PresetChatOptionsDto> MapDomainToDto(IEnumerable<PresetChatOptions> entities)
    {
        return [.. entities.Select(mapper.MapDomainToDto)];
    }
}
