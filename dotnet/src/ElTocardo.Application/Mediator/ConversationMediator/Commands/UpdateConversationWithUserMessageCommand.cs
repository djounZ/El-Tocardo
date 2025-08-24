using ElTocardo.Application.Mediator.Common.Commands;
using Microsoft.Extensions.AI;

namespace ElTocardo.Application.Mediator.ConversationMediator.Commands;

public sealed record UpdateConversationWithUserMessageCommand(
    string Key,
    ChatMessage UserMessage) : UpdateCommandBase<string>(Key);

public sealed record UpdateConversationWithChatResponseCommand(
    string Key,
    ChatResponse ChatResponse) : UpdateCommandBase<string>(Key);



