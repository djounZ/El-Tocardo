using ElTocardo.Application.Mediator.McpServerConfigurationMediator.Commands;
using ElTocardo.Domain.Mediator.Common.Mappers;
using ElTocardo.Domain.Mediator.McpServerConfigurationMediator.Entities;

namespace ElTocardo.Application.Mediator.McpServerConfigurationMediator.Mappers;

public class McpServerConfigurationDomainUpdateCommandMapper :AbstractDomainUpdateCommandMapper<McpServerConfiguration, Guid, string, UpdateMcpServerCommand>
{

    public override void UpdateFromCommand(McpServerConfiguration domain, UpdateMcpServerCommand command)
    {
        domain.Update(
            command.Category,
            command.Command,
            command.Arguments,
            command.EnvironmentVariables,
            command.Endpoint,
            command.TransportType);
    }
}
