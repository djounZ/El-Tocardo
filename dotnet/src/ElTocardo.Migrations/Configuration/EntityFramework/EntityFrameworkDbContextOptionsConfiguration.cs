using ElTocardo.Infrastructure.EntityFramework.Mediator.Common.Configurations;
using Microsoft.EntityFrameworkCore;
using static ElTocardo.ServiceDefaults.Constants;

namespace ElTocardo.Migrations.Configuration.EntityFramework;

public sealed class EntityFrameworkDbContextOptionsConfiguration : ElTocardoDbContextOptionsConfiguration
{
    public override void Configure(IServiceProvider serviceProvider, DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString(PostgresDatabaseResourceName));
        optionsBuilder.UseOpenIddict();
    }

}
