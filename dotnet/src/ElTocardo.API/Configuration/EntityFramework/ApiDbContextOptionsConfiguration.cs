using ElTocardo.Infrastructure.EntityFramework.Mediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using static ElTocardo.ServiceDefaults.Constants;

namespace ElTocardo.API.Configuration.EntityFramework;

public sealed class ApiDbContextOptionsConfiguration : IDbContextOptionsConfiguration<ApplicationDbContext>
{
    public void Configure(IServiceProvider serviceProvider, DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString(PostgresDatabaseResourceName));
    }

}
