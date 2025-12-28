namespace ElTocardo.Application.Mediator.UserExternalTokenMediator.Commands;

public sealed record CreateUserExternalTokenCommand(
    string UserId,
    string Provider,
    string Value);
