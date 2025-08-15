using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.AI.Contents;

public sealed record UsageContentDto(
    IList<AiAnnotationDto>? Annotations,
    [property: JsonPropertyName("details")]
    UsageDetailsDto DetailsDto)
    : AiContentDto(Annotations);
