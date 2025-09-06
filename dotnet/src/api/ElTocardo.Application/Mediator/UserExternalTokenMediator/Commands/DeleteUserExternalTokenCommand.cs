using ElTocardo.Domain.Mediator.Common.Commands;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;

namespace ElTocardo.Application.Mediator.UserExternalTokenMediator.Commands;

public sealed record DeleteUserExternalTokenCommand(UserExternalTokenKey Key) : DeleteCommandBase<UserExternalTokenKey>(Key);
