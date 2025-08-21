using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.Common.Models;
using ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Commands;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Handlers.Commands;

/// <summary>
/// Handler for ConfirmPasswordResetCommand.
/// </summary>
public class ConfirmPasswordResetCommandHandler : ICommandHandler<ConfirmPasswordResetCommand>
{
    private readonly UserManager<ApplicationUser> userManager;

    public ConfirmPasswordResetCommandHandler(UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<VoidResult> HandleAsync(ConfirmPasswordResetCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null)
        {
            return new ArgumentException("User not found");
        }
        var result = await userManager.ResetPasswordAsync(user, command.Token, command.NewPassword);
        if (result.Succeeded)
        {
            return VoidResult.Success;
        }
        return string.Join(", ", result.Errors.Select(e => e.Description));
    }
}
