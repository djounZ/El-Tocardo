using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;

public class ReasoningOutputMapper(
    ILogger<ReasoningOutputMapper> logger
) : IDomainEntityMapper<ReasoningOutput, ReasoningOutputEnumDto>
{

    public ReasoningOutputEnumDto ToApplication(ReasoningOutput domainItem)
    {
        switch (domainItem)
        {
            case ReasoningOutput.None:
                return ReasoningOutputEnumDto.None;
            case ReasoningOutput.Summary:
                return ReasoningOutputEnumDto.Summary;
            case ReasoningOutput.Full:
                return ReasoningOutputEnumDto.Full;
            default:
                var argumentOutOfRangeException = new ArgumentOutOfRangeException(nameof(domainItem), domainItem, null);
                logger.LogError(argumentOutOfRangeException,"Failed {@Item}", domainItem);
                throw argumentOutOfRangeException;
        }
    }
    public ReasoningOutput ToDomain(ReasoningOutputEnumDto applicationItem)
    {
        switch (applicationItem)
        {
            case ReasoningOutputEnumDto.None:
                return ReasoningOutput.None;
            case ReasoningOutputEnumDto.Summary:
                return ReasoningOutput.Summary;
            case ReasoningOutputEnumDto.Full:
                return ReasoningOutput.Full;
            default:
                var argumentOutOfRangeException = new ArgumentOutOfRangeException(nameof(applicationItem), applicationItem, null);
                logger.LogError(argumentOutOfRangeException,"Failed {@Item}", applicationItem);
                throw argumentOutOfRangeException;
        }
    }
}