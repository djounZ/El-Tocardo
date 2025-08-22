using ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Commands;
using Microsoft.AspNetCore.Identity;
using ElTocardo.Application.Mediator.Common.Handlers;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Handlers.Commands;

/// <summary>
/// Handler for AuthenticateUserCommand.
/// </summary>
public class AuthenticateUserCommandHandler(ILogger<AuthenticateUserCommandHandler> logger, UserManager<ApplicationUser> userManager)
    : CommandHandlerBase<AuthenticateUserCommand, string>(logger)
{
    protected override async Task<string> HandleAsyncImplementation(AuthenticateUserCommand command, CancellationToken cancellationToken = default)
    {
        // Find user by username
        var user = await userManager.FindByNameAsync(command.Username)
                   ?? throw new ArgumentException($"Invalid username {command.Username}", nameof(command));

        // Check password
        var isValid = await userManager.CheckPasswordAsync(user, command.Password);
        return !isValid ? throw new ArgumentException("Invalid username or password") :
            // Success: return user ID (or you could return a JWT here)
            user.Id;

        // Alternatively, you could return a JWT token here if you have JWT generation logic
    }
}
