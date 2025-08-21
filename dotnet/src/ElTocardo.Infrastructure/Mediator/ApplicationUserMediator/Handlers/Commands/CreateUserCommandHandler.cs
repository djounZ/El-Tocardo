using ElTocardo.Application.Mediator.Common.Handlers;
using ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.Mediator.ApplicationUserMediator.Handlers.Commands;

public class CreateUserCommandHandler(ILogger<CreateUserCommandHandler> logger, UserManager<ApplicationUser> userManager): CommandHandlerBase<CreateUserCommand, string>(logger)
{
    protected override async Task<string> HandleAsyncImplementation(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Registering User: {UserName}", command.Username);
        var user = new ApplicationUser(command.Username) {  Email = command.Email };
        var result = await userManager.CreateAsync(user, command.Password);
        if (result.Succeeded)
        {
            var findByNameAsync = await userManager.FindByNameAsync(command.Username);
            return findByNameAsync!.Id;
        }
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        logger.LogError("Failed to register user: {UserName}. Errors: {Errors}", command.Username, errors);
        throw new InvalidOperationException($"Failed to register user: {command.Username}. Errors: {errors}");
    }
}
