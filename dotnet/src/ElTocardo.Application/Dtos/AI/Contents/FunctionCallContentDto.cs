using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.AI.Contents;

public sealed record FunctionCallContentDto(
    IList<AiAnnotationDto>? Annotations,
    [property: JsonPropertyName("call_id")]
    string CallId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("arguments")]
    IDictionary<string, object?>? Arguments)
    : AiContentDto(Annotations);
