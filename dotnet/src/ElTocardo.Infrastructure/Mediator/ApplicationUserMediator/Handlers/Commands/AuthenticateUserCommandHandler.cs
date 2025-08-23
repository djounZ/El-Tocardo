using ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Commands;
using Microsoft.AspNetCore.Identity;
using ElTocardo.Application.Mediator.Common.Handlers;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Handlers.Commands;

/// <summary>
/// Handler for AuthenticateUserCommand.
/// </summary>
public class AuthenticateUserCommandHandler(ILogger<AuthenticateUserCommandHandler> logger, UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager)
    : CommandHandlerBase<AuthenticateUserCommand>(logger)
{
    protected override async Task HandleAsyncImplementation(AuthenticateUserCommand command, CancellationToken cancellationToken = default)
    {
        // Find user by username
        var user = await userManager.FindByNameAsync(command.Username)
                   ?? throw new ArgumentException($"Invalid username {command.Username}", nameof(command));

        // Check password
        var signInResult = await signInManager.CheckPasswordSignInAsync(user, command.Password, lockoutOnFailure: false);
        if (!signInResult.Succeeded)
        {
            throw new InvalidOperationException(signInResult.ToString());
        }
    }
}
