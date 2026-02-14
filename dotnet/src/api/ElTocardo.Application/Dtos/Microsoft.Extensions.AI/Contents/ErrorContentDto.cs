using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;

public sealed record ErrorContentDto(
    IList<AiAnnotationDto>? Annotations,
    [property: JsonPropertyName("message")]
    string Message)
    : AiContentDto(Annotations);
