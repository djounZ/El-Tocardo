using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Tools;

public class AIToolMapper(ILogger<AIToolMapper> logger) : IDomainEntityMapper<AITool, AiToolDto>
{

    public AiToolDto ToApplication(AITool domainItem)
    {
        var notImplementedException = new NotImplementedException();
        logger.LogError(notImplementedException, "To Do");
        throw notImplementedException;
    }
    public AITool ToDomain(AiToolDto applicationItem)
    {
        var notImplementedException = new NotImplementedException();
        logger.LogError(notImplementedException, "To Do");
        throw notImplementedException;
    }
}
