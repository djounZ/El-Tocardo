using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI.Contents;

public class CitationAnnotationMapper(
    IDomainEntityMapper<AnnotatedRegion, AnnotatedRegionDto> annotatedRegionMapper
) : IDomainEntityMapper<CitationAnnotation, CitationAnnotationDto>
{
    public CitationAnnotationDto ToApplication(CitationAnnotation domainItem)
    {
        var annotatedRegions = domainItem.AnnotatedRegions?.Select(annotatedRegionMapper.ToApplication).ToList();
        var title = domainItem.Title;
        var url = domainItem.Url ?? throw new ArgumentNullException(nameof(domainItem.Url));
        var fileId = domainItem.FileId;
        var toolName = domainItem.ToolName;
        var snippet = domainItem.Snippet;

        var result = new CitationAnnotationDto(
            annotatedRegions,
            title,
            url,
            fileId,
            toolName,
            snippet
        );

        return result;
    }

    public CitationAnnotation ToDomain(CitationAnnotationDto applicationItem)
    {
        var annotatedRegions = applicationItem.AnnotatedRegions?.Select(annotatedRegionMapper.ToDomain).ToList();
        var title = applicationItem.Title;
        var url = applicationItem.Url;
        var fileId = applicationItem.FileId;
        var toolName = applicationItem.ToolName;
        var snippet = applicationItem.Snippet;

        var result = new CitationAnnotation
        {
            AnnotatedRegions = annotatedRegions,
            Title = title,
            Url = url,
            FileId = fileId,
            ToolName = toolName,
            Snippet = snippet
        };

        return result;
    }
}
