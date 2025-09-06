using ElTocardo.Domain.Mediator.Common.Commands;
using ElTocardo.Domain.Mediator.UserExternalTokenMediator.Entities;

namespace ElTocardo.Application.Mediator.UserExternalTokenMediator.Commands;

public sealed record UpdateUserExternalTokenCommand(
    UserExternalTokenKey Key,
    string Value) : UpdateCommandBase<UserExternalTokenKey>(Key);
