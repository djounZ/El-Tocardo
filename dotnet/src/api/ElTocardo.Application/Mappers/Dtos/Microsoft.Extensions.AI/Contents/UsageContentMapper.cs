using ElTocardo.Application.Dtos.Microsoft.Extensions.AI;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Contents;

public class UsageContentMapper(
    IDomainEntityMapper<AIAnnotation, AiAnnotationDto> aiAnnotationMapper,
    IDomainEntityMapper<UsageDetails, UsageDetailsDto> usageDetailsMapper
) : IDomainEntityMapper<UsageContent, UsageContentDto>
{
    public UsageContentDto ToApplication(UsageContent domainItem)
    {
        var annotations = domainItem.Annotations?.Select(aiAnnotationMapper.ToApplication).ToList();
        var details = usageDetailsMapper.ToApplication(domainItem.Details);

        var result = new UsageContentDto(
            annotations,
            details
        );

        return result;
    }

    public UsageContent ToDomain(UsageContentDto applicationItem)
    {
        var annotations = applicationItem.Annotations?.Select(aiAnnotationMapper.ToDomain).ToList();
        var details = usageDetailsMapper.ToDomain(applicationItem.DetailsDto);

        var result = new UsageContent(details)
        {
            Annotations = annotations
        };

        return result;
    }
}
