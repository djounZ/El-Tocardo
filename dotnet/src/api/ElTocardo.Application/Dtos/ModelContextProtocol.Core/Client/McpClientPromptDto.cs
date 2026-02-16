using System.Text.Json;
using System.Text.Json.Serialization;
using ElTocardo.Application.Dtos.ModelContextProtocol.Core.Protocol;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core.Client;

public sealed record McpClientPromptDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("arguments")]
    IList<PromptArgumentDto>? Arguments,
    [property: JsonPropertyName("_meta")] JsonElement? Meta);