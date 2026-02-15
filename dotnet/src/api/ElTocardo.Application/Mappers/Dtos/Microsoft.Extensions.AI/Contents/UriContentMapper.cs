using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Contents;

public class UriContentMapper(
    IDomainEntityMapper<AIAnnotation, AiAnnotationDto> aiAnnotationMapper
) : IDomainEntityMapper<UriContent, UriContentDto>
{
    public UriContentDto ToApplication(UriContent domainItem)
    {
        var annotations = domainItem.Annotations?.Select(aiAnnotationMapper.ToApplication).ToList();
        var uri = domainItem.Uri;
        var mediaType = domainItem.MediaType;

        var result = new UriContentDto(
            annotations,
            uri,
            mediaType
        );

        return result;
    }

    public UriContent ToDomain(UriContentDto applicationItem)
    {
        var annotations = applicationItem.Annotations?.Select(aiAnnotationMapper.ToDomain).ToList();
        var uri = applicationItem.Uri;
        var mediaType = applicationItem.MediaType;

        var result = new UriContent(uri, mediaType)
        {
            Annotations = annotations
        };

        return result;
    }
}
