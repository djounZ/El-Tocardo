using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;

public sealed record FunctionResultContentDto(
    IList<AiAnnotationDto>? Annotations,
    [property: JsonPropertyName("call_id")]
    string CallId,
    [property: JsonPropertyName("result")] object? Result)
    : AiContentDto(Annotations);
