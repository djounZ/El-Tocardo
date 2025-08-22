using ElTocardo.Application.Services;
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.Common.Models;
using ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Commands;
using System.Security.Claims;

namespace ElTocardo.Infrastructure.Services;

/// <summary>
/// Service for user management operations, orchestrating command and query handlers.
/// </summary>
public class UserService(
    ICommandHandler<CreateUserCommand> createUserHandler,
    ICommandHandler<AuthenticateUserCommand, string> authenticateUserHandler,
    ICommandHandler<InitiatePasswordResetCommand, string> initiatePasswordResetHandler,
    ICommandHandler<ConfirmPasswordResetCommand> confirmPasswordResetHandler,
    ICommandHandler<UnregisterUserCommand> unregisterUserHandler)
    : IUserService
{
    /// <summary>
    /// Registers a new user by delegating to the CreateUserCommand handler.
    /// </summary>
    public async Task<VoidResult> RegisterUserAsync(string username, string email, string password, CancellationToken cancellationToken = default)
    {
        var command = new CreateUserCommand(username, email, password);
        return await createUserHandler.HandleAsync(command, cancellationToken);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token by delegating to the AuthenticateUserCommand handler.
    /// </summary>
    public async Task<Result<string>> AuthenticateUserAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var command = new AuthenticateUserCommand(username, password);
        return await authenticateUserHandler.HandleAsync(command, cancellationToken);
    }

    /// <summary>
    /// Initiates a password reset by delegating to the InitiatePasswordResetCommand handler.
    /// </summary>
    public async Task<Result<string>> InitiatePasswordResetAsync(string email, CancellationToken cancellationToken = default)
    {
        var command = new InitiatePasswordResetCommand(email);
        return await initiatePasswordResetHandler.HandleAsync(command, cancellationToken);
    }

    /// <summary>
    /// Confirms a password reset by delegating to the ConfirmPasswordResetCommand handler.
    /// </summary>
    public async Task<VoidResult> ConfirmPasswordResetAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default)
    {
        var command = new ConfirmPasswordResetCommand(email, token, newPassword);
        return await confirmPasswordResetHandler.HandleAsync(command, cancellationToken);
    }

    /// <summary>
    /// Unregisters (deletes) the current user by delegating to the UnregisterUserCommand handler.
    /// </summary>
    public async Task<VoidResult> UnregisterUserAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var command = new UnregisterUserCommand(user);
        return await unregisterUserHandler.HandleAsync(command, cancellationToken);
    }
}
