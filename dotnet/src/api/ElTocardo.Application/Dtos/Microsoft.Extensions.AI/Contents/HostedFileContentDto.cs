using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;

public sealed record HostedFileContentDto(
    IList<AiAnnotationDto>? Annotations,
    [property: JsonPropertyName("file_id")] string FileId,
    [property: JsonPropertyName("media_type")]
    string? MediaType,
    [property: JsonPropertyName("name")] string? Name) : AiContentDto(Annotations);
