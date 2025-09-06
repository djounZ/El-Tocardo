using System.Security.Claims;
using ElTocardo.Domain.Models;

namespace ElTocardo.Application.Services;



public interface IUserEndpointService
{
    /// <summary>
    /// Registers a new user by delegating to the CreateUserCommand handler.
    /// </summary>
    public Task<VoidResult> RegisterUserAsync(string username, string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user and returns a JWT token by delegating to the AuthenticateUserCommand handler.
    /// </summary>
    public Task<VoidResult> AuthenticateUserAsync(string username, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates a password reset by delegating to the InitiatePasswordResetCommand handler.
    /// </summary>
    public Task<Result<string>> InitiatePasswordResetAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms a password reset by delegating to the ConfirmPasswordResetCommand handler.
    /// </summary>
    public Task<VoidResult> ConfirmPasswordResetAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters (deletes) the current user by delegating to the UnregisterUserCommand handler.
    /// </summary>
    public Task<VoidResult> UnregisterUserAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);
}
