using ElTocardo.Domain.Mediator.Common.Queries;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;

namespace ElTocardo.Application.Mediator.UserExternalTokenMediator.Queries;

public sealed record GetUserExternalTokenByKeyQuery(UserExternalTokenKey Key) : GetByKeyQueryBase<UserExternalTokenKey>(Key);
