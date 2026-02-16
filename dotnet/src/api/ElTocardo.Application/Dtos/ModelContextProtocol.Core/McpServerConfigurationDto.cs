using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core;

public sealed record McpServerConfigurationDto(
    [property: JsonPropertyName("servers")]
    IDictionary<string, McpServerConfigurationItemDto> Servers);