using ElTocardo.Domain.Mediator.ApplicationUserMediator;
using Microsoft.AspNetCore.Identity;

namespace ElTocardo.Infrastructure.EntityFramework.Mediator.ApplicationUserMediator;

public class ApplicationUser : IdentityUser, IApplicationUser
{


    private ApplicationUser() { } // EF Core constructor

    public ApplicationUser(string userName) : base(userName)
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        ValidateApplicationUser();
    }

    private void ValidateApplicationUser()
    {

        if (string.IsNullOrWhiteSpace(UserName))
        {
            throw new ArgumentException("User name cannot be null or empty", nameof(UserName));
        }
    }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public string GetKey()
    {
        return UserName ?? throw new ArgumentNullException(nameof(UserName), "UserName should not be null");
    }
}
