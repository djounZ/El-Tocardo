using ElTocardo.Domain.Mediator.Common.Entities;
using Microsoft.AspNetCore.Identity;

namespace ElTocardo.Infrastructure.Mediator.EntityFramework.ApplicationUserMediator;

public class ApplicationUser : IdentityUser, IEntity<string, string>
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
