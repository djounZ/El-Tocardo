using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Contents;

public class HostedVectorStoreContentMapper(
    IDomainEntityMapper<AIAnnotation, AiAnnotationDto> aiAnnotationMapper
) : IDomainEntityMapper<HostedVectorStoreContent, HostedVectorStoreContentDto>
{
    public HostedVectorStoreContentDto ToApplication(HostedVectorStoreContent domainItem)
    {
        var annotations = domainItem.Annotations?.Select(aiAnnotationMapper.ToApplication).ToList();
        var vectorStoreId = domainItem.VectorStoreId;

        var result = new HostedVectorStoreContentDto(
            annotations,
            vectorStoreId,
            null
        );

        return result;
    }

    public HostedVectorStoreContent ToDomain(HostedVectorStoreContentDto applicationItem)
    {
        var annotations = applicationItem.Annotations?.Select(aiAnnotationMapper.ToDomain).ToList();
        var vectorStoreId = applicationItem.VectorStoreId;

        var result = new HostedVectorStoreContent(vectorStoreId)
        {
            Annotations = annotations
        };

        return result;
    }
}
