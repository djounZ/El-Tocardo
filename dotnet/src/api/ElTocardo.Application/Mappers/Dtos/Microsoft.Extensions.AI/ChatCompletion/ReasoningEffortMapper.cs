using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;

public class ReasoningEffortMapper(
    ILogger<ReasoningEffortMapper> logger
) : IDomainEntityMapper<ReasoningEffort, ReasoningEffortEnumDto>
{

    public ReasoningEffortEnumDto ToApplication(ReasoningEffort domainItem)
    {
        switch (domainItem)
        {
            case ReasoningEffort.None:
                return ReasoningEffortEnumDto.None;
            case ReasoningEffort.ExtraHigh:
                return ReasoningEffortEnumDto.ExtraHigh;
            case ReasoningEffort.High:
                return ReasoningEffortEnumDto.High;
            case ReasoningEffort.Medium:
                return ReasoningEffortEnumDto.Medium;
            case ReasoningEffort.Low:
                return ReasoningEffortEnumDto.Low;
            default:
                var argumentOutOfRangeException = new ArgumentOutOfRangeException(nameof(domainItem), domainItem, null);
                logger.LogError(argumentOutOfRangeException,"Failed {@Item}", domainItem);
                throw argumentOutOfRangeException;
        }
    }
    public ReasoningEffort ToDomain(ReasoningEffortEnumDto applicationItem)
    {
        switch (applicationItem)
        {
            case ReasoningEffortEnumDto.None:
                return ReasoningEffort.None;
            case ReasoningEffortEnumDto.ExtraHigh:
                return ReasoningEffort.ExtraHigh;
            case ReasoningEffortEnumDto.High:
                return ReasoningEffort.High;
            case ReasoningEffortEnumDto.Medium:
                return ReasoningEffort.Medium;
            case ReasoningEffortEnumDto.Low:
                return ReasoningEffort.Low;
            default:
                var argumentOutOfRangeException = new ArgumentOutOfRangeException(nameof(applicationItem), applicationItem, null);
                logger.LogError(argumentOutOfRangeException,"Failed {@Item}", applicationItem);
                throw argumentOutOfRangeException;
        }
    }
}