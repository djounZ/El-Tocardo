namespace ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Commands;

/// <summary>
/// Command to confirm a password reset for a user.
/// </summary>
public sealed record ConfirmPasswordResetCommand(string Email, string Token, string NewPassword);
