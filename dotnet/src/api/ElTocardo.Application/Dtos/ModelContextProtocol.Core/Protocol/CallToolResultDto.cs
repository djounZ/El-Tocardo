using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.ModelContextProtocol.Core.Protocol;

public sealed record CallToolResultDto(
    [property: JsonPropertyName("content")]
    IList<ContentBlockDto> Content,
    [property: JsonPropertyName("structured_content")]
    JsonNode? StructuredContent,
    [property: JsonPropertyName("isError")]
    bool? IsError
);
