using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;

public sealed record ChatResponseFormatJsonDto(
    [property: JsonPropertyName("schema")] string? Schema,
    [property: JsonPropertyName("schema_name")]
    string? SchemaName,
    [property: JsonPropertyName("schema_description")]
    string? SchemaDescription
) : ChatResponseFormatDto;
