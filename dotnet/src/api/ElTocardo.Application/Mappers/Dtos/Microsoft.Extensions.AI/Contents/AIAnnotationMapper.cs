using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Contents;

public class AIAnnotationMapper(
    ILogger<AIAnnotationMapper> logger,
    IDomainEntityMapper<CitationAnnotation, CitationAnnotationDto> citationAnnotationMapper
): IDomainEntityMapper<AIAnnotation, AiAnnotationDto>
{
    public AiAnnotationDto ToApplication(AIAnnotation domainItem)
    {
        switch (domainItem)
        {
            case CitationAnnotation citationAnnotation:
                return citationAnnotationMapper.ToApplication(citationAnnotation);
            default:
                var notSupportedException = new NotSupportedException($"{domainItem.GetType()} is not supported");
                logger.LogError(notSupportedException, "Failed {@Item}", domainItem);
                throw notSupportedException;
        }
    }


    public AIAnnotation ToDomain(AiAnnotationDto applicationItem)
    {
        switch (applicationItem)
        {
            case CitationAnnotationDto citationAnnotation:
                return citationAnnotationMapper.ToDomain(citationAnnotation);
            default:
                var notSupportedException = new NotSupportedException($"{applicationItem.GetType()} is not supported");
                logger.LogError(notSupportedException, "Failed {@Item}", applicationItem);
                throw notSupportedException;
        }
    }
}
