using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Contents;

public class HostedFileContentMapper(
    IDomainEntityMapper<AIAnnotation, AiAnnotationDto> aiAnnotationMapper
) : IDomainEntityMapper<HostedFileContent, HostedFileContentDto>
{
    public HostedFileContentDto ToApplication(HostedFileContent domainItem)
    {
        var annotations = domainItem.Annotations?.Select(aiAnnotationMapper.ToApplication).ToList();
        var fileId = domainItem.FileId;
        var mediaType = domainItem.MediaType;
        var name = domainItem.Name;

        var result = new HostedFileContentDto(
            annotations,
            fileId,
            mediaType,
            name
        );

        return result;
    }

    public HostedFileContent ToDomain(HostedFileContentDto applicationItem)
    {
        var annotations = applicationItem.Annotations?.Select(aiAnnotationMapper.ToDomain).ToList();
        var fileId = applicationItem.FileId;
        var mediaType = applicationItem.MediaType;
        var name = applicationItem.Name;

        var result = new HostedFileContent(fileId)
        {
            Annotations = annotations,
            MediaType = mediaType,
            Name = name
        };

        return result;
    }
}
