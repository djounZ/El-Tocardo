using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mediator.ConversationMediator.Commands;

public sealed record CreateConversationCommand(
    string Title,
    string? Description,
    ChatMessage UserMessage,
    ChatOptions? ChatOptions,
    string? Provider);
