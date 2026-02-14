using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;

public sealed record UriContentDto(
    IList<AiAnnotationDto>? Annotations,
    [property: JsonPropertyName("uri")] Uri Uri,
    [property: JsonPropertyName("media_type")]
    string MediaType) : AiContentDto(Annotations);
