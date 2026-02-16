using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core.Protocol;

public sealed record AnnotationsDto(
    [property: JsonPropertyName("audience")]
    IList<McpClientRoleDto>? Audience,
    [property: JsonPropertyName("priority")]
    float? Priority,
    [property: JsonPropertyName("lastModified")]
    DateTimeOffset? LastModified);