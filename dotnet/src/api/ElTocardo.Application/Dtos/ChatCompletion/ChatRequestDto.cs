using ElTocardo.Application.Dtos.Configuration;
using ElTocardo.Application.Dtos.Microsoft.Extensions.AI.ChatCompletion;

namespace ElTocardo.Application.Dtos.ChatCompletion;

public sealed record ChatRequestDto(
    IEnumerable<ChatMessageDto> Messages,
    AiProviderEnumDto? Provider = null,
    ChatOptionsDto? Options = null
);
