using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Provider;

public record AiProviderDto(
    [property: JsonPropertyName("name")] AiProviderEnumDto Name,
    [property: JsonPropertyName("models")] IList<AiProviderAiModelDto> Models);
