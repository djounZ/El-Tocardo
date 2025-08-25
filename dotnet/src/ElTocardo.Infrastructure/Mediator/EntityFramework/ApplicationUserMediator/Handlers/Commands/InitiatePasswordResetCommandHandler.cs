using ElTocardo.Application.Mediator.Common.Handlers;
using ElTocardo.Infrastructure.Mediator.EntityFramework.ApplicationUserMediator.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Mediator.EntityFramework.ApplicationUserMediator.Handlers.Commands;

/// <summary>
/// Handler for InitiatePasswordResetCommand.
/// </summary>
public class InitiatePasswordResetCommandHandler(ILogger<InitiatePasswordResetCommandHandler> logger, UserManager<ApplicationUser> userManager)
    : CommandHandlerBase<InitiatePasswordResetCommand, string>(logger)
{
    protected  override async Task<string> HandleAsyncImplementation(InitiatePasswordResetCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(command.Email)
                   ?? throw new ArgumentException($"User not found : {command.Email}", nameof(command));
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        return token;
    }
}
