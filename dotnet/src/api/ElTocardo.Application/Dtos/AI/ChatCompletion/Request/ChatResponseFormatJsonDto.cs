using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.AI.ChatCompletion.Request;

public sealed record ChatResponseFormatJsonDto(
    [property: JsonPropertyName("schema")] string? Schema,
    [property: JsonPropertyName("schema_name")]
    string? SchemaName,
    [property: JsonPropertyName("schema_description")]
    string? SchemaDescription
) : ChatResponseFormatDto;
