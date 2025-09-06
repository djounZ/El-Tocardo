namespace ElTocardo.Infrastructure.EntityFramework.Mediator.ApplicationUserMediator.Commands;

public sealed record CreateUserCommand(string Username, string Email, string Password);
