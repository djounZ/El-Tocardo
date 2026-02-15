using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Contents;

public class AnnotatedRegionMapper(
    ILogger<AnnotatedRegionMapper> logger,
    IDomainEntityMapper<TextSpanAnnotatedRegion, TextSpanAnnotatedRegionDto> textSpanAnnotatedRegionMapper
) : IDomainEntityMapper<AnnotatedRegion, AnnotatedRegionDto>
{
    public AnnotatedRegionDto ToApplication(AnnotatedRegion domainItem)
    {
        switch (domainItem)
        {
            case TextSpanAnnotatedRegion textSpanAnnotatedRegion:
                return textSpanAnnotatedRegionMapper.ToApplication(textSpanAnnotatedRegion);
            default:
                var notSupportedException = new NotSupportedException($"{domainItem.GetType()} is not supported");
                logger.LogError(notSupportedException, "Failed {@Item}", domainItem);
                throw notSupportedException;
        }
    }

    public AnnotatedRegion ToDomain(AnnotatedRegionDto applicationItem)
    {
        switch (applicationItem)
        {
            case TextSpanAnnotatedRegionDto textSpanAnnotatedRegion:
                return textSpanAnnotatedRegionMapper.ToDomain(textSpanAnnotatedRegion);
            default:
                var notSupportedException = new NotSupportedException($"{applicationItem.GetType()} is not supported");
                logger.LogError(notSupportedException, "Failed {@Item}", applicationItem);
                throw notSupportedException;
        }
    }
}
