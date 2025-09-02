using ElTocardo.Application.Mediator.Common.Handlers;
using ElTocardo.Infrastructure.EntityFramework.Mediator.ApplicationUserMediator.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.ApplicationUserMediator.Handlers.Commands;

public class CreateUserCommandHandler(ILogger<CreateUserCommandHandler> logger, UserManager<ApplicationUser> userManager): CommandHandlerBase<CreateUserCommand>(logger)
{
    protected override async Task HandleAsyncImplementation(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Registering User: {UserName}", command.Username);
        var user = new ApplicationUser(command.Username) {  Email = command.Email };
        var result = await userManager.CreateAsync(user, command.Password);
        if (result.Succeeded)
        {
            return;
        }
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        var invalidOperationException = new InvalidOperationException($"Failed to register user: {command.Username}. Errors: {errors}");
        invalidOperationException.Data.Add("IdentityResult", result);
        logger.LogError(invalidOperationException,"Failed to register user: {UserName}. Errors: {Errors}", command.Username, errors);
        throw invalidOperationException;
    }
}
