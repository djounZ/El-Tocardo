using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.AI.ChatCompletion.Request;

public record AiToolDto(
    [property: JsonPropertyName("name")] string Name);
