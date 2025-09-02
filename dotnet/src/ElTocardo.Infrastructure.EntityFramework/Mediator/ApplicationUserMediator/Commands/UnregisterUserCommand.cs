using System.Security.Claims;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.ApplicationUserMediator.Commands;

/// <summary>
/// Command to unregister (delete) a user.
/// </summary>
public sealed record UnregisterUserCommand(ClaimsPrincipal User);
