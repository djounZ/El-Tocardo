using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Contents;

public class AIContentMapper(ILogger<AIContentMapper> logger): IDomainEntityMapper<AIContent, AiContentDto>
{

    public AiContentDto ToApplication(AIContent domainItem)
    {
        var notImplementedException = new NotImplementedException();
        logger.LogError(notImplementedException, "To Do");
        throw notImplementedException;
    }
    public AIContent ToDomain(AiContentDto applicationItem)
    {
        var notImplementedException = new NotImplementedException();
        logger.LogError(notImplementedException, "To Do");
        throw notImplementedException;
    }
}
