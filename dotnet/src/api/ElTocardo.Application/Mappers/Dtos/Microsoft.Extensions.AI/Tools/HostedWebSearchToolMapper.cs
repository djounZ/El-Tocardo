using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Tools;

public class HostedWebSearchToolMapper : IDomainEntityMapper<HostedWebSearchTool, HostedWebSearchToolDto>
{
    public HostedWebSearchToolDto ToApplication(HostedWebSearchTool domainItem)
    {
        var name = domainItem.Name;
        var description = domainItem.Description;

        var result = new HostedWebSearchToolDto(
            name,
            description
        );

        return result;
    }

    public HostedWebSearchTool ToDomain(HostedWebSearchToolDto applicationItem)
    {

        var result = new HostedWebSearchTool();

        return result;
    }
}
