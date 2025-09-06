using ElTocardo.Domain.Mediator.Common.Commands;

namespace ElTocardo.Application.Mediator.ConversationMediator.Commands;

public sealed record DeleteConversationCommand(string Key) : DeleteCommandBase<string>(Key);
