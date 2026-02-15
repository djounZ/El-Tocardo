using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;

public sealed record FunctionCallContentDto(
    IList<AiAnnotationDto>? Annotations,
    [property: JsonPropertyName("call_id")]
    string CallId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("arguments")]
    IDictionary<string, object?>? Arguments,
    [property: JsonPropertyName("informational_only")] bool InformationalOnly)
    : AiContentDto(Annotations);
