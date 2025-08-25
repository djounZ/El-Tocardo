using System.Security.Claims;

namespace ElTocardo.Infrastructure.Mediator.EntityFramework.ApplicationUserMediator.Commands;

/// <summary>
/// Command to unregister (delete) a user.
/// </summary>
public sealed record UnregisterUserCommand(ClaimsPrincipal User);
