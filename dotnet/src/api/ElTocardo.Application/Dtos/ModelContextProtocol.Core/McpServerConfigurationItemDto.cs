using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core;

public sealed record McpServerConfigurationItemDto(
    [property: JsonPropertyName("category")]
    string? Category,
    [property: JsonPropertyName("command")]
    string? Command,
    [property: JsonPropertyName("args")] IList<string>? Arguments,
    [property: JsonPropertyName("env")] IDictionary<string, string?>? EnvironmentVariables,
    [property: JsonPropertyName("url")] Uri? Endpoint,
    [property: JsonPropertyName("type")] McpServerTransportTypeDto Type = McpServerTransportTypeDto.Stdio);
