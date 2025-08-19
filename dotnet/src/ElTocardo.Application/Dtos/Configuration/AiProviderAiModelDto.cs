using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Configuration;

public record AiProviderAiModelDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name);
