using ElTocardo.Infrastructure.EntityFramework.Mediator.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ElTocardo.Migrations.Configuration.EntityFramework;

public sealed class EntityFrameworkDbContextOptionsConfiguration : ElTocardoDbContextOptionsConfiguration
{
    public override void Configure(IServiceProvider serviceProvider, DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("el-tocardo-db-postgres"));
        optionsBuilder.UseOpenIddict();
    }

}
