using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core.Protocol;

public sealed record BlobResourceContentsDto(
    string Uri,
    string? MediaType,
    JsonObject? Meta,
    [property: JsonPropertyName("blob")] string Blob
) : ResourceContentsDto(Uri, MediaType, Meta);