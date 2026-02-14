using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Tools;

public record AiToolDto(
    [property: JsonPropertyName("name")] string Name);
