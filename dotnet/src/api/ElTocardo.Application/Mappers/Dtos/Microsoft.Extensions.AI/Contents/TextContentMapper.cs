using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Contents;

public class TextContentMapper(
    IDomainEntityMapper<AIAnnotation, AiAnnotationDto> aiAnnotationMapper
) : IDomainEntityMapper<TextContent, TextContentDto>
{
    public TextContentDto ToApplication(TextContent domainItem)
    {
        var annotations = domainItem.Annotations?.Select(aiAnnotationMapper.ToApplication).ToList();
        var text = domainItem.Text;

        var result = new TextContentDto(
            annotations,
            text
        );

        return result;
    }

    public TextContent ToDomain(TextContentDto applicationItem)
    {
        var annotations = applicationItem.Annotations?.Select(aiAnnotationMapper.ToDomain).ToList();
        var text = applicationItem.Text;

        var result = new TextContent(text)
        {
            Annotations = annotations
        };

        return result;
    }
}
