using ElTocardo.Application.Dtos.Microsoft.Extensions.AI;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI;

public class UsageDetailsMapper : IDomainEntityMapper<UsageDetails, UsageDetailsDto>
{
    public UsageDetailsDto ToApplication(UsageDetails domainItem)
    {
        var inputTokenCount = domainItem.InputTokenCount;
        var outputTokenCount = domainItem.OutputTokenCount;
        var totalTokenCount = domainItem.TotalTokenCount;
        var cachedInputTokenCount = domainItem.CachedInputTokenCount;
        var reasoningTokenCount = domainItem.ReasoningTokenCount;
        var additionalCounts = domainItem.AdditionalCounts;

        var result = new UsageDetailsDto(
            inputTokenCount,
            outputTokenCount,
            totalTokenCount,
            cachedInputTokenCount,
            reasoningTokenCount,
            additionalCounts
        );

        return result;
    }

    public UsageDetails ToDomain(UsageDetailsDto applicationItem)
    {
        var inputTokenCount = applicationItem.InputTokenCount;
        var outputTokenCount = applicationItem.OutputTokenCount;
        var totalTokenCount = applicationItem.TotalTokenCount;
        var cachedInputTokenCount = applicationItem.CachedInputTokenCount;
        var reasoningTokenCount = applicationItem.ReasoningTokenCount;

        var result = new UsageDetails
        {
            InputTokenCount = inputTokenCount,
            OutputTokenCount = outputTokenCount,
            TotalTokenCount = totalTokenCount,
            CachedInputTokenCount = cachedInputTokenCount,
            ReasoningTokenCount = reasoningTokenCount
        };
        return result;
    }
}
