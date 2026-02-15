using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Functions;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Tools;

public class AIToolMapper(
    ILogger<AIToolMapper> logger,
    IDomainEntityMapper<HostedFileSearchTool, HostedFileSearchToolDto> hostedFileSearchToolMapper,
    IDomainEntityMapper<HostedWebSearchTool, HostedWebSearchToolDto> hostedWebSearchToolMapper,
    IDomainEntityMapper<AIFunctionDeclaration, DelegatingAiFunctionDeclarationDto> delegatingAiFunctionDeclarationMapper,
    IDomainEntityMapper<DelegatingAIFunction, DelegatingAiFunctionDto> delegatingAiFunctionMapper
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
            case DelegatingAIFunction delegatingAiFunction:
                return delegatingAiFunctionMapper.ToApplication(delegatingAiFunction);
            case AIFunctionDeclaration aiFunctionDeclaration:
                return delegatingAiFunctionDeclarationMapper.ToApplication(aiFunctionDeclaration);
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
            case DelegatingAiFunctionDto delegatingAiFunction:
                return delegatingAiFunctionMapper.ToDomain(delegatingAiFunction);
            case DelegatingAiFunctionDeclarationDto aiFunctionDeclaration:
                return delegatingAiFunctionDeclarationMapper.ToDomain(aiFunctionDeclaration);
            default:
                var notSupportedException = new NotSupportedException($"{applicationItem.GetType()} is not supported");
                logger.LogError(notSupportedException, "Failed {@Item}", applicationItem);
                throw notSupportedException;
        }
    }
}
