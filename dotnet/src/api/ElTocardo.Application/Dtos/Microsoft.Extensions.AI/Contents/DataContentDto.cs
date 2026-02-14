using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;

public sealed record DataContentDto(
    IList<AiAnnotationDto>? Annotations,
    [property: JsonPropertyName("uri")] Uri Uri,
    [property: JsonPropertyName("media_type")]
    string? MediaType,
    [property: JsonPropertyName("name")] string? Name) : AiContentDto(Annotations);
