using ElTocardo.Application.Mediator.Common.Interfaces;
using ElTocardo.Application.Mediator.Common.Models;
using ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Commands;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Handlers.Commands;

/// <summary>
/// Handler for InitiatePasswordResetCommand.
/// </summary>
public class InitiatePasswordResetCommandHandler : ICommandHandler<InitiatePasswordResetCommand, string>
{
    private readonly UserManager<ApplicationUser> userManager;

    public InitiatePasswordResetCommandHandler(UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<Result<string>> HandleAsync(InitiatePasswordResetCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null)
        {
            return new ArgumentException("User not found");
        }
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        return token;
    }
}
