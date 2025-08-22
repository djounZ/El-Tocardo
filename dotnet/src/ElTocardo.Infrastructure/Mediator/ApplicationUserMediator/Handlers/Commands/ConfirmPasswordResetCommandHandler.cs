using ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Commands;
using Microsoft.AspNetCore.Identity;
using ElTocardo.Application.Mediator.Common.Handlers;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Handlers.Commands;

/// <summary>
/// Handler for ConfirmPasswordResetCommand.
/// </summary>
public class ConfirmPasswordResetCommandHandler(ILogger<ConfirmPasswordResetCommandHandler> logger,  UserManager<ApplicationUser> userManager)
    : CommandHandlerBase<ConfirmPasswordResetCommand>(logger)
{
    protected override async Task HandleAsyncImplementation(ConfirmPasswordResetCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(command.Email) ?? throw new ArgumentException($"Email not found: {command.Email}", nameof(command));
        var result = await userManager.ResetPasswordAsync(user, command.Token, command.NewPassword);

        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        var invalidOperationException =
            new InvalidOperationException($"Failed to reset password for email: {command.Email}. Errors: {errors}");
        invalidOperationException.Data.Add("IdentityResult", result);
        logger.LogError(invalidOperationException, "Failed to reset password for email: {Email}. Errors: {Errors}",
            command.Email, errors);
        throw invalidOperationException;
    }
}
