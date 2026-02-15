using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mappers.Dtos.Microsoft.Extensions.AI;

public record AiChatClientRequest(
    IEnumerable<ChatMessage> Messages,
    ChatOptions? Options = null);
