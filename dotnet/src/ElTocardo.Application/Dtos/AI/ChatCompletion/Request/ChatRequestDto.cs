using ElTocardo.Application.Dtos.Provider;

namespace ElTocardo.Application.Dtos.AI.ChatCompletion.Request;

public sealed record ChatRequestDto(
    IEnumerable<ChatMessageDto> Messages,
    AiProviderEnumDto? Provider = null,
    ChatOptionsDto? Options = null
);
