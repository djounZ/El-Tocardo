using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.AI.Contents;

public sealed record TextReasoningContentDto(
    IList<AiAnnotationDto>? Annotations,
    [property: JsonPropertyName("text")] string? Text)
    : AiContentDto(Annotations);
