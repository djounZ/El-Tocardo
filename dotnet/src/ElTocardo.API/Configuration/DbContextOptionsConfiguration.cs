using ElTocardo.Infrastructure.Mediator.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ElTocardo.API.Configuration;

public sealed class DbContextOptionsConfiguration : IDbContextOptionsConfiguration<ApplicationDbContext>
{
    public void Configure(IServiceProvider serviceProvider, DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("el-tocardo-db-postgres"));
        optionsBuilder.UseOpenIddict();
    }
}
