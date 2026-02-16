using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core.Protocol;

public sealed record ResourceLinkBlockDto(
    AnnotationsDto? Annotations,
    [property: JsonPropertyName("uri")] string Uri,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("media_type")]
    string? MediaType,
    [property: JsonPropertyName("size")] long? Size
) : ContentBlockDto(Annotations);