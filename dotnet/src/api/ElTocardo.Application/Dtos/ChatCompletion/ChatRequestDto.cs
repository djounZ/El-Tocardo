using System.Text.Json.Serialization;
using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;

namespace ElTocardo.Application.Dtos.ChatCompletion;

public sealed record ChatRequestDto(
    [property: JsonPropertyName("messages")] IEnumerable<ChatMessageDto> Messages,
    [property: JsonPropertyName("provider")] AiProviderEnumDto? Provider = null,
    [property: JsonPropertyName("options")] ChatOptionsDto? Options = null
);
