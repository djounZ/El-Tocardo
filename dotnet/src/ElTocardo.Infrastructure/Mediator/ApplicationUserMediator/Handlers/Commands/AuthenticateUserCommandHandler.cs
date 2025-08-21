
using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.Common.Models;
using ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Commands;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Handlers.Commands;

/// <summary>
/// Handler for AuthenticateUserCommand.
/// </summary>
public class AuthenticateUserCommandHandler : ICommandHandler<AuthenticateUserCommand, string>
{
    private readonly UserManager<ApplicationUser> userManager;

    public AuthenticateUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<Result<string>> HandleAsync(AuthenticateUserCommand command, CancellationToken cancellationToken = default)
    {
        // Find user by username
        var user = await userManager.FindByNameAsync(command.Username);
        if (user is null)
        {
            return new ArgumentException("Invalid username or password");
        }

        // Check password
        var isValid = await userManager.CheckPasswordAsync(user, command.Password);
        if (!isValid)
        {
            return new ArgumentException("Invalid username or password");
        }

        // Success: return user ID (or you could return a JWT here)
        return user.Id;
        // Alternatively, you could return a JWT token here if you have JWT generation logic
    }
}
