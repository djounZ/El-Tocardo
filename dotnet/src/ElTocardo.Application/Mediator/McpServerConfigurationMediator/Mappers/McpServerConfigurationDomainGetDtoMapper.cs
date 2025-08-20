using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.ValueObjects;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Mappers;

public class McpServerConfigurationDomainGetDtoMapper : AbstractDomainGetDtoMapper<McpServerConfiguration, string, McpServerConfigurationItemDto>
{
    public override  McpServerConfigurationItemDto MapDomainToDto(McpServerConfiguration configuration)
    {
        return new McpServerConfigurationItemDto(
            configuration.Category,
            configuration.Command,
            configuration.Arguments,
            configuration.EnvironmentVariables,
            configuration.Endpoint,
            ToDto(configuration.TransportType));
    }

    private static McpServerTransportTypeDto ToDto(McpServerTransportType transportType)
    {
        return transportType switch
        {
            McpServerTransportType.Stdio => McpServerTransportTypeDto.Stdio,
            McpServerTransportType.Http => McpServerTransportTypeDto.Http,
            _ => throw new ArgumentOutOfRangeException(nameof(transportType), transportType, null)
        };
    }
}
