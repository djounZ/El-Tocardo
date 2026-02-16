using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core.Protocol;

public sealed record AudioContentBlockDto(
    AnnotationsDto? Annotations,
    [property: JsonPropertyName("data")] string Data,
    [property: JsonPropertyName("media_type")]
    string MediaType,
    [property: JsonPropertyName("_meta")] JsonObject? Meta
) : ContentBlockDto(Annotations);