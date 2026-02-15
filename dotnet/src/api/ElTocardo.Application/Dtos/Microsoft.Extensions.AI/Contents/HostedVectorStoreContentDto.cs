using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;

public sealed record HostedVectorStoreContentDto(
    IList<AiAnnotationDto>? Annotations,
    [property: JsonPropertyName("vector_store_id")] string VectorStoreId,
    [property: JsonPropertyName("name")] string? Name) : AiContentDto(Annotations);
