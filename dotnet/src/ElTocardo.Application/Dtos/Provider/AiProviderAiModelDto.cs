using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Provider;

public record AiProviderAiModelDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name);
