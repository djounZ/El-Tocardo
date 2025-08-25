using ElTocardo.Application.Mediator.Common.Queries;

namespace ElTocardo.Application.Mediator.ConversationMediator.Queries;

public sealed record GetConversationByIdQuery(string Id) : GetByIdQueryBase<string>(Id);
