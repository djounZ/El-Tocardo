using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum McpServerTransportTypeDto
{
    [JsonStringEnumMemberName("stdio")] Stdio = 1,
    [JsonStringEnumMemberName("http")] Http = 2
}

public sealed record McpServerConfigurationItemDto(
    [property: JsonPropertyName("category")]
    string? Category,
    [property: JsonPropertyName("command")]
    string? Command,
    [property: JsonPropertyName("args")] IList<string>? Arguments,
    [property: JsonPropertyName("env")] IDictionary<string, string?>? EnvironmentVariables,
    [property: JsonPropertyName("url")] Uri? Endpoint,
    [property: JsonPropertyName("type")] McpServerTransportTypeDto Type = McpServerTransportTypeDto.Stdio);

public sealed record McpServerConfigurationDto(
    [property: JsonPropertyName("servers")]
    IDictionary<string, McpServerConfigurationItemDto> Servers);

public sealed record McpClientToolDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("input_schema")]
    JsonElement InputSchema,
    [property: JsonPropertyName("output_schema")]
    JsonElement? OutputSchema);

public sealed record PromptArgumentDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("required")]
    bool? Required);

public sealed record McpClientPromptDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("arguments")]
    IList<PromptArgumentDto>? Arguments,
    [property: JsonPropertyName("_meta")] JsonElement? Meta);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum McpClientRoleDto
{
    [JsonStringEnumMemberName("assistant")]
    Assistant = 2,
    [JsonStringEnumMemberName("user")] Tool = 4
}

public sealed record AnnotationsDto(
    [property: JsonPropertyName("audience")]
    IList<McpClientRoleDto>? Audience,
    [property: JsonPropertyName("priority")]
    float? Priority,
    [property: JsonPropertyName("lastModified")]
    DateTimeOffset? LastModified);

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

public sealed record McpClientResourceDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("uri")] string? Uri,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("mimeType")]
    string? MimeType,
    [property: JsonPropertyName("annotations")]
    AnnotationsDto? Annotations,
    [property: JsonPropertyName("size")] long? Size,
    [property: JsonPropertyName("_meta")] JsonElement? Meta);

public sealed record McpClientDto(
    [property: JsonPropertyName("tools")] IList<McpClientToolDto> Tools,
    [property: JsonPropertyName("prompts")]
    IList<McpClientPromptDto> Prompts,
    [property: JsonPropertyName("resource_templates")]
    IList<McpClientResourceTemplateDto> ResourceTemplates,
    [property: JsonPropertyName("resources")]
    IList<McpClientResourceDto> Resources
);

public sealed record McpClientToolRequestDto(
    [property: JsonPropertyName("server_name")]
    string ServerName,
    [property: JsonPropertyName("tool_name")]
    string ToolName,
    [property: JsonPropertyName("arguments")]
    IReadOnlyDictionary<string, object?>? Arguments
);

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(TextContentBlockDto), "text")]
[JsonDerivedType(typeof(ImageContentBlockDto), "image")]
[JsonDerivedType(typeof(AudioContentBlockDto), "audio")]
[JsonDerivedType(typeof(EmbeddedResourceBlockDto), "resource")]
[JsonDerivedType(typeof(ResourceLinkBlockDto), "resource_link")]
public abstract record ContentBlockDto(
    [property: JsonPropertyName("annotations")]
    AnnotationsDto? Annotations
);

public sealed record TextContentBlockDto(
    AnnotationsDto? Annotations,
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("_meta")] JsonObject? Meta
) : ContentBlockDto(Annotations);

public sealed record ImageContentBlockDto(
    AnnotationsDto? Annotations,
    [property: JsonPropertyName("data")] string Data,
    [property: JsonPropertyName("media_type")]
    string MediaType,
    [property: JsonPropertyName("_meta")] JsonObject? Meta
) : ContentBlockDto(Annotations);

public sealed record AudioContentBlockDto(
    AnnotationsDto? Annotations,
    [property: JsonPropertyName("data")] string Data,
    [property: JsonPropertyName("media_type")]
    string MediaType,
    [property: JsonPropertyName("_meta")] JsonObject? Meta
) : ContentBlockDto(Annotations);

public sealed record EmbeddedResourceBlockDto(
    AnnotationsDto? Annotations,
    [property: JsonPropertyName("resource")]
    ResourceContentsDto Resource,
    [property: JsonPropertyName("_meta")] JsonObject? Meta
) : ContentBlockDto(Annotations);

public sealed record ResourceLinkBlockDto(
    AnnotationsDto? Annotations,
    [property: JsonPropertyName("uri")] string Uri,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("media_type")]
    string? MediaType,
    [property: JsonPropertyName("size")] long? Size
) : ContentBlockDto(Annotations);

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(BlobResourceContentsDto), "blob")]
[JsonDerivedType(typeof(TextResourceContentsDto), "text")]
public abstract record ResourceContentsDto(
    [property: JsonPropertyName("uri")] string Uri,
    [property: JsonPropertyName("media_type")]
    string? MediaType,
    [property: JsonPropertyName("_meta")] JsonObject? Meta
);

public sealed record BlobResourceContentsDto(
    string Uri,
    string? MediaType,
    JsonObject? Meta,
    [property: JsonPropertyName("blob")] string Blob
) : ResourceContentsDto(Uri, MediaType, Meta);

public sealed record TextResourceContentsDto(
    string Uri,
    string? MediaType,
    JsonObject? Meta,
    [property: JsonPropertyName("text")] string Text
) : ResourceContentsDto(Uri, MediaType, Meta);

public sealed record CallToolResultDto(
    [property: JsonPropertyName("content")]
    IList<ContentBlockDto> Content,
    [property: JsonPropertyName("structured_content")]
    JsonNode? StructuredContent,
    [property: JsonPropertyName("isError")]
    bool? IsError
);
