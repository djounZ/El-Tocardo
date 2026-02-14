using ElTocardo.Application.Dtos.Microsoft.Extensions.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI;

public class UsageDetailsMapper(ILogger<UsageDetailsMapper> logger) : IDomainEntityMapper<UsageDetails, UsageDetailsDto>
{

    public UsageDetailsDto ToApplication(UsageDetails domainItem)
    {
        var notImplementedException = new NotImplementedException();
        logger.LogError(notImplementedException, "To Do");
        throw notImplementedException;
    }
    public UsageDetails ToDomain(UsageDetailsDto applicationItem)
    {
        var notImplementedException = new NotImplementedException();
        logger.LogError(notImplementedException, "To Do");
        throw notImplementedException;
    }
}
