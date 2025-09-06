namespace ElTocardo.Infrastructure.EntityFramework.Mediator.ApplicationUserMediator.Commands;

/// <summary>
/// Command to initiate a password reset for a user.
/// </summary>
public sealed record InitiatePasswordResetCommand(string Email);
