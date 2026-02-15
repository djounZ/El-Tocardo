using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Contents;

public class TextSpanAnnotatedRegionMapper : IDomainEntityMapper<TextSpanAnnotatedRegion, TextSpanAnnotatedRegionDto>
{
    public TextSpanAnnotatedRegionDto ToApplication(TextSpanAnnotatedRegion domainItem)
    {
        var startIndex = domainItem.StartIndex;
        var endIndex = domainItem.EndIndex;

        var result = new TextSpanAnnotatedRegionDto(
            startIndex,
            endIndex
        );

        return result;
    }

    public TextSpanAnnotatedRegion ToDomain(TextSpanAnnotatedRegionDto applicationItem)
    {
        var startIndex = applicationItem.StartIndex;
        var endIndex = applicationItem.EndIndex;

        var result = new TextSpanAnnotatedRegion { StartIndex = startIndex, EndIndex = endIndex };

        return result;
    }
}
