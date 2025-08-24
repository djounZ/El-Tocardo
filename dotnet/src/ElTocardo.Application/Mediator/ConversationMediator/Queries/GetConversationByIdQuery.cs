using ElTocardo.Application.Mediator.Common.Queries;

namespace ElTocardo.Application.Mediator.ConversationMediator.Queries;

public sealed record GetConversationByIdQuery(string Key) : GetByKeyQueryBase<string>(Key);
