using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;

public sealed record CitationAnnotationDto(
    IList<AnnotatedRegionDto>? AnnotatedRegions,
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("url")] Uri Url,
    [property: JsonPropertyName("file_id")]
    string? FileId,
    [property: JsonPropertyName("tool_name")]
    string? ToolName,
    [property: JsonPropertyName("snippet")]
    string? Snippet) : AiAnnotationDto(AnnotatedRegions);
