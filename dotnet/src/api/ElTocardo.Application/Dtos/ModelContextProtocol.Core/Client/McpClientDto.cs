using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core.Client;

public sealed record McpClientDto(
    [property: JsonPropertyName("tools")] IList<McpClientToolDto> Tools,
    [property: JsonPropertyName("prompts")]
    IList<McpClientPromptDto> Prompts,
    [property: JsonPropertyName("resource_templates")]
    IList<McpClientResourceTemplateDto> ResourceTemplates,
    [property: JsonPropertyName("resources")]
    IList<McpClientResourceDto> Resources
);