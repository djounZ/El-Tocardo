using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core.Protocol;

public sealed record TextResourceContentsDto(
    string Uri,
    string? MediaType,
    JsonObject? Meta,
    [property: JsonPropertyName("text")] string Text
) : ResourceContentsDto(Uri, MediaType, Meta);