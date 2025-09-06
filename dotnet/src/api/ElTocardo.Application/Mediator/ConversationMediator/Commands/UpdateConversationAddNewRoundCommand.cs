using ElTocardo.Domain.Mediator.Common.Commands;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mediator.ConversationMediator.Commands;

public sealed record UpdateConversationAddNewRoundCommand(
    string Id,
    ChatMessage UserMessage,
    ChatOptions? Options,
    string? Provider) : UpdateCommandBase<string>(Id);
