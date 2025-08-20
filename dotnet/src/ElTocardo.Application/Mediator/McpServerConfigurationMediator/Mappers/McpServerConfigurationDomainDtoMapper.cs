using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Commands;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.ValueObjects;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Mappers;



public class McpServerConfigurationDomainCommandMapper
{
    public McpServerConfiguration CreateFromCommand(CreateMcpServerCommand command)
    {
        return new McpServerConfiguration(
            command.ServerName,
            command.Category,
            command.Command,
            command.Arguments,
            command.EnvironmentVariables,
            command.Endpoint,
            command.TransportType);
    }

    public void UpdateFromCommand(McpServerConfiguration configuration, UpdateMcpServerCommand command)
    {
        configuration.Update(
            command.Category,
            command.Command,
            command.Arguments,
            command.EnvironmentVariables,
            command.Endpoint,
            command.TransportType);
    }
}


public class McpServerConfigurationDomainDtoMapper
{



    public Dictionary<string, McpServerConfigurationItemDto> MapDomainToDto(IEnumerable<McpServerConfiguration> configurations)
    {
        return configurations.ToDictionary(
            config => config.ServerName,
            MapDomainToDto);
    }
    public  McpServerConfigurationItemDto MapDomainToDto(McpServerConfiguration configuration)
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
