using System.Security.Claims;
using ElTocardo.Application.Mediator.Common.Handlers;
using ElTocardo.Infrastructure.EntityFramework.Mediator.ApplicationUserMediator.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.ApplicationUserMediator.Handlers.Commands;

/// <summary>
/// Handler for UnregisterUserCommand.
/// </summary>
public class UnregisterUserCommandHandler(ILogger<UnregisterUserCommandHandler> logger, UserManager<ApplicationUser> userManager)
    : CommandHandlerBase<UnregisterUserCommand>(logger)
{
    protected override async Task HandleAsyncImplementation(UnregisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var userId = command.User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? throw new ArgumentException($"Unauthorized User {command.User} - No UserId claim found", nameof(command));
        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new ArgumentException($"User {command.User} not found", nameof(command));

        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return;
        }
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        var invalidOperationException =
            new InvalidOperationException($"Failed to unregister user: {command.User}. Errors: {errors}");
        invalidOperationException.Data.Add("IdentityResult", result);
        logger.LogError(invalidOperationException, "Failed to unregister user: {User}. Errors: {Errors}",
            command.User, errors);
        throw invalidOperationException;
    }
}
