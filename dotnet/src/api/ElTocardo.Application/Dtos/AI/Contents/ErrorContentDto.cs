using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.AI.Contents;

public sealed record ErrorContentDto(
    IList<AiAnnotationDto>? Annotations,
    [property: JsonPropertyName("message")]
    string Message)
    : AiContentDto(Annotations);
