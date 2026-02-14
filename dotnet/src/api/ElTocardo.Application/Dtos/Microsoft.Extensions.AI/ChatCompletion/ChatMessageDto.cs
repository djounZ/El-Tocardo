using System.Text.Json.Serialization;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.Contents;

namespace ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;

public sealed record ChatMessageDto(
    [property: JsonPropertyName("role")] ChatRoleEnumDto Role,
    [property: JsonPropertyName("contents")]
    IList<AiContentDto> Contents
);
