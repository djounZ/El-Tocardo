namespace ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Commands;

public sealed record CreateUserCommand(string Username, string Email, string Password);
