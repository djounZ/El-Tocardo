using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElTocardo.Authorization.EntityFramework.Infrastructure;

public class AuthorizationDbContext(
    DbContextOptions options)  : IdentityDbContext<IdentityUser>(options), IDataProtectionKeyContext
{

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
}
