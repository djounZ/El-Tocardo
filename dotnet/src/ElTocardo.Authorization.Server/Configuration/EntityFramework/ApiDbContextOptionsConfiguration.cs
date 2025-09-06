using ElTocardo.Authorization.EntityFramework.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using static ElTocardo.ServiceDefaults.Constants;

namespace ElTocardo.Authorization.Server.Configuration.EntityFramework;

public sealed class ApiDbContextOptionsConfiguration : IDbContextOptionsConfiguration<AuthorizationDbContext>
{
    public void Configure(IServiceProvider serviceProvider, DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString(PostgresDatabaseResourceName));
        optionsBuilder.UseOpenIddict();
    }

}
