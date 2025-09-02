using ElTocardo.Application.Mediator.Common.Handlers;
using ElTocardo.Infrastructure.EntityFramework.Mediator.ApplicationUserMediator.Queries;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.ApplicationUserMediator.Handlers.Queries;

public class GetAllUsersQueryHandler(ILogger<GetAllUsersQueryHandler> logger, UserManager<ApplicationUser> userManager): QueryHandlerBase<GetAllUsersQuery, ApplicationUser[]>(logger)
{
    protected override async Task<ApplicationUser[]> HandleAsyncImplementation(GetAllUsersQuery query, CancellationToken cancellationToken = default)
    {

        logger.LogInformation("Getting all users");

        var admins = await userManager.GetUsersInRoleAsync("ADMIN");

        logger.LogInformation("Found {Count} users", admins.Count);
        return [.. admins];
    }
}
