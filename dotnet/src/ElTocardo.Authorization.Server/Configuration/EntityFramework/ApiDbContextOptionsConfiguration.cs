using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ElTocardo.Authorization.Server.Configuration.EntityFramework;

public sealed class ApiDbContextOptionsConfiguration : IDbContextOptionsConfiguration<DbContext>
{
    public void Configure(IServiceProvider serviceProvider, DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("el-tocardo-db-postgres"));
        optionsBuilder.UseOpenIddict();
    }

}
