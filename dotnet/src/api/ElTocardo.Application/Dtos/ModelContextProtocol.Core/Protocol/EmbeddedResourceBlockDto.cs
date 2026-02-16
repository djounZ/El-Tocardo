using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core.Protocol;

public sealed record EmbeddedResourceBlockDto(
    AnnotationsDto? Annotations,
    [property: JsonPropertyName("resource")]
    ResourceContentsDto Resource,
    [property: JsonPropertyName("_meta")] JsonObject? Meta
) : ContentBlockDto(Annotations);