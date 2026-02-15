using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Tools;

public class AIToolMapper(
    ILogger<AIToolMapper> logger,
    IDomainEntityMapper<HostedFileSearchTool, HostedFileSearchToolDto> hostedFileSearchToolMapper,
    IDomainEntityMapper<HostedWebSearchTool, HostedWebSearchToolDto> hostedWebSearchToolMapper
) : IDomainEntityMapper<AITool, AbstractAiToolDto>
{
    public AbstractAiToolDto ToApplication(AITool domainItem)
    {
        switch (domainItem)
        {
            case HostedFileSearchTool hostedFileSearchTool:
                return hostedFileSearchToolMapper.ToApplication(hostedFileSearchTool);
            case HostedWebSearchTool hostedWebSearchTool:
                return hostedWebSearchToolMapper.ToApplication(hostedWebSearchTool);
            default:
                return new AiToolDto(domainItem.Name, domainItem.Description);
        }
    }

    public AITool ToDomain(AbstractAiToolDto applicationItem)
    {
        switch (applicationItem)
        {
            case HostedFileSearchToolDto hostedFileSearchTool:
                return hostedFileSearchToolMapper.ToDomain(hostedFileSearchTool);
            case HostedWebSearchToolDto hostedWebSearchTool:
                return hostedWebSearchToolMapper.ToDomain(hostedWebSearchTool);
            default:
                var notSupportedException = new NotSupportedException($"{applicationItem.GetType()} is not supported");
                logger.LogError(notSupportedException, "Failed {@Item}", applicationItem);
                throw notSupportedException;
        }
    }
}
