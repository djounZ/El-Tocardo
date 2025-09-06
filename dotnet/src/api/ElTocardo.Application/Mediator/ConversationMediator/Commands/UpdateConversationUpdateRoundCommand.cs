using ElTocardo.Domain.Mediator.Commands;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mediator.ConversationMediator.Commands;

public sealed record UpdateConversationUpdateRoundCommand(
    string Id,
    ChatResponse ChatResponse) : UpdateCommandBase<string>(Id);
