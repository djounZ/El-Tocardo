using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core;

public sealed record McpClientToolRequestDto(
    [property: JsonPropertyName("server_name")]
    string ServerName,
    [property: JsonPropertyName("tool_name")]
    string ToolName,
    [property: JsonPropertyName("arguments")]
    IReadOnlyDictionary<string, object?>? Arguments
);