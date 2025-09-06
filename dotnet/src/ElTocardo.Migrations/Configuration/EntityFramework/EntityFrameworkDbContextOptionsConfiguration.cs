using ElTocardo.Infrastructure.EntityFramework.Mediator;
using ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using static ElTocardo.ServiceDefaults.Constants;

namespace ElTocardo.Migrations.Configuration.EntityFramework;

public sealed class EntityFrameworkDbContextOptionsConfiguration : IDbContextOptionsConfiguration<MigrationDbContext>
{
    public void Configure(IServiceProvider serviceProvider, DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString(PostgresDatabaseResourceName));
        optionsBuilder.UseOpenIddict();
    }

}
