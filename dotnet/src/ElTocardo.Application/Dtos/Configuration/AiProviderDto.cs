using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Configuration;

public record AiProviderDto(
    [property: JsonPropertyName("name")] AiProviderEnumDto Name,
    [property: JsonPropertyName("models")] IList<AiProviderAiModelDto> Models);
