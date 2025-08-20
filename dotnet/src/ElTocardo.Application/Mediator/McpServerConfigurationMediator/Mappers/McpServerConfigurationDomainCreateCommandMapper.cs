using ElTocardo.Application.Mediator.Common.Mappers;
using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Commands;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Mappers;

public class McpServerConfigurationDomainCreateCommandMapper :AbstractDomainCreateCommandMapper<McpServerConfiguration, string, CreateMcpServerCommand>
{
    public override McpServerConfiguration CreateFromCommand(CreateMcpServerCommand command)
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
}