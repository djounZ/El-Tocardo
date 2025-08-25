namespace ElTocardo.Infrastructure.Mediator.EntityFramework.ApplicationUserMediator.Commands;

public sealed record CreateUserCommand(string Username, string Email, string Password);
