using ElTocardo.Application.Mediator.Common.Commands;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mediator.ConversationMediator.Commands;

public sealed record UpdateConversationUpdateRoundCommand(
    string Id,
    ChatResponse ChatResponse) : UpdateCommandBase<string>(Id);
