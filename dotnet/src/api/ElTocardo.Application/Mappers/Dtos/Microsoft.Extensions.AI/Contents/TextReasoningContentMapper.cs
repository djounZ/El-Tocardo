using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Contents;

public class TextReasoningContentMapper(
    IDomainEntityMapper<AIAnnotation, AiAnnotationDto> aiAnnotationMapper
) : IDomainEntityMapper<TextReasoningContent, TextReasoningContentDto>
{
    public TextReasoningContentDto ToApplication(TextReasoningContent domainItem)
    {
        var annotations = domainItem.Annotations?.Select(aiAnnotationMapper.ToApplication).ToList();
        var text = domainItem.Text;

        var result = new TextReasoningContentDto(
            annotations,
            text
        );

        return result;
    }

    public TextReasoningContent ToDomain(TextReasoningContentDto applicationItem)
    {
        var annotations = applicationItem.Annotations?.Select(aiAnnotationMapper.ToDomain).ToList();
        var text = applicationItem.Text;

        var result = new TextReasoningContent(text)
        {
            Annotations = annotations
        };

        return result;
    }
}
