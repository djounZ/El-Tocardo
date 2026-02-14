using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.ChatCompletion;

public class ReasoningOptionsMapper(
    IDomainEntityMapper<ReasoningEffort, ReasoningEffortEnumDto> reasoningEffortMapper,
    IDomainEntityMapper<ReasoningOutput, ReasoningOutputEnumDto> reasoningOutputMapper
) : IDomainEntityMapper<ReasoningOptions, ReasoningOptionsDto>
{

    public ReasoningOptionsDto ToApplication(ReasoningOptions domainItem)
    {
        var effort = reasoningEffortMapper.ToApplication(domainItem.Effort);
        var output = reasoningOutputMapper.ToApplication(domainItem.Output);
        return new ReasoningOptionsDto(effort, output);
    }
    public ReasoningOptions ToDomain(ReasoningOptionsDto applicationItem)
    {
        var effort = reasoningEffortMapper.ToDomain(applicationItem.Effort);
        var outputEnumDto = reasoningOutputMapper.ToDomain(applicationItem.Output);
        return new ReasoningOptions
        {
            Effort = effort,
            Output = outputEnumDto
        };
    }
}
