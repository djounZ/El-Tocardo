using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Dtos.ModelContextProtocol.Core;
using ElTocardo.Domain.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Mappers;

public class McpServerConfigurationDomainGetAllDtoMapper(McpServerConfigurationDomainGetDtoMapper mapper) : AbstractDomainGetAllDtoMapper<McpServerConfiguration, Guid, string, Dictionary<string, McpServerConfigurationItemDto>>
{
    public override Dictionary<string, McpServerConfigurationItemDto> MapDomainToDto(IEnumerable<McpServerConfiguration> configurations)
    {
        return configurations.ToDictionary(
            config => config.ServerName,
            mapper.MapDomainToDto);
    }
}
