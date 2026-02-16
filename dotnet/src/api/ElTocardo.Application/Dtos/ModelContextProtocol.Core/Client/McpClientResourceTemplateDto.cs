using System.Text.Json;
using System.Text.Json.Serialization;
using ElTocardo.Application.Dtos.ModelContextProtocol.Core.Protocol;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core.Client;

public sealed record McpClientResourceTemplateDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("uriTemplate")]
    string? UriTemplate,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("mimeType")]
    string? MimeType,
    [property: JsonPropertyName("annotations")]
    AnnotationsDto? Annotations,
    [property: JsonPropertyName("_meta")] JsonElement? Meta);