using System.Text.Json.Serialization;
using ElTocardo.Application.Dtos.AI.Contents;

namespace ElTocardo.Application.Dtos.AI.ChatCompletion.Request;

public sealed record ChatMessageDto(
    [property: JsonPropertyName("role")] ChatRoleEnumDto Role,
    [property: JsonPropertyName("contents")]
    IList<AiContentDto> Contents
);
