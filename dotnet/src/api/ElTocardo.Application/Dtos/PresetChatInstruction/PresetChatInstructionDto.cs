using System.Text.Json.Serialization;

namespace ElTocardo.Application.Dtos.PresetChatInstruction
{
    public record PresetChatInstructionDto(
        [property: JsonPropertyName("name")]
        string Name,
        [property: JsonPropertyName("description")]
        string Description,
        [property: JsonPropertyName("content_type")]
        string ContentType,
        [property: JsonPropertyName("content")]
        string Content);
}
