using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Contents;

public class DataContentMapper(
    IDomainEntityMapper<AIAnnotation, AiAnnotationDto> aiAnnotationMapper
): IDomainEntityMapper<DataContent, DataContentDto>
{

    public DataContentDto ToApplication(DataContent domainItem)
    {
        var annotations = domainItem.Annotations?.Select(aiAnnotationMapper.ToApplication).ToList();
        var uri = new Uri(domainItem.Uri);
        var mediaType = domainItem.MediaType;
        var name = domainItem.Name;


        var result = new DataContentDto(
            annotations,
            uri,
            mediaType,
            name
        );

        return result;
    }


    public DataContent ToDomain(DataContentDto applicationItem)
    {
        var annotations = applicationItem.Annotations?.Select(aiAnnotationMapper.ToDomain).ToList();
        var uri = applicationItem.Uri.ToString();
        var mediaType = applicationItem.MediaType;
        var name = applicationItem.Name;

        var result = new DataContent
            (uri, mediaType)
            {
                Annotations = annotations,
                Name = name,
            };

        return result;
    }
}
