using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.Common.Models;
using ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Commands;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Handlers.Commands;

/// <summary>
/// Handler for UnregisterUserCommand.
/// </summary>
public class UnregisterUserCommandHandler : ICommandHandler<UnregisterUserCommand>
{
    private readonly UserManager<ApplicationUser> userManager;

    public UnregisterUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<VoidResult> HandleAsync(UnregisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var userId = command.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return new ArgumentException("Unauthorized");
        }
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return new ArgumentException("User not found");
        }
        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return VoidResult.Success;
        }
        return string.Join(", ", result.Errors.Select(e => e.Description));
    }
}
