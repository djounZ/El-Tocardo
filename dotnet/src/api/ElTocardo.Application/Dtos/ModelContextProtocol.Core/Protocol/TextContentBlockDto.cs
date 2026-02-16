using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core.Protocol;

public sealed record TextContentBlockDto(
    AnnotationsDto? Annotations,
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("_meta")] JsonObject? Meta
) : ContentBlockDto(Annotations);
