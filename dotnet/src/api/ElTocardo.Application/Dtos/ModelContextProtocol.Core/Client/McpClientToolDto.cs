using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core.Client;

public sealed record McpClientToolDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("input_schema")]
    JsonElement InputSchema,
    [property: JsonPropertyName("output_schema")]
    JsonElement? OutputSchema);