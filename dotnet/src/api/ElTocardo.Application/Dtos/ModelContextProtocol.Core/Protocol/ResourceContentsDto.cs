using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core.Protocol;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(BlobResourceContentsDto), "blob")]
[JsonDerivedType(typeof(TextResourceContentsDto), "text")]
public abstract record ResourceContentsDto(
    [property: JsonPropertyName("uri")] string Uri,
    [property: JsonPropertyName("media_type")]
    string? MediaType,
    [property: JsonPropertyName("_meta")] JsonObject? Meta
);