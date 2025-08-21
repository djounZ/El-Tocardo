namespace ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Commands;

/// <summary>
/// Command to authenticate a user (login).
/// </summary>
public sealed record AuthenticateUserCommand(string Username, string Password);
