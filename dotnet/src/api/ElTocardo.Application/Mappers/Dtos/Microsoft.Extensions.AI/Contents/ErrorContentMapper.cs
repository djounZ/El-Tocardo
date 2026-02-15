using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Contents;

public class ErrorContentMapper(
    IDomainEntityMapper<AIAnnotation, AiAnnotationDto> aiAnnotationMapper
) : IDomainEntityMapper<ErrorContent, ErrorContentDto>
{
    public ErrorContentDto ToApplication(ErrorContent domainItem)
    {
        var annotations = domainItem.Annotations?.Select(aiAnnotationMapper.ToApplication).ToList();
        var message = domainItem.Message;

        var result = new ErrorContentDto(
            annotations,
            message
        );

        return result;
    }

    public ErrorContent ToDomain(ErrorContentDto applicationItem)
    {
        var annotations = applicationItem.Annotations?.Select(aiAnnotationMapper.ToDomain).ToList();
        var message = applicationItem.Message;

        var result = new ErrorContent(message)
        {
            Annotations = annotations
        };

        return result;
    }
}
